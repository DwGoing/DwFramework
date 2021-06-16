#!/bin/bash
usage="Usage:\n-c|--configuration <CONFIGURATION>\n-o|--output <OUTPUT_DIRECTORY>\n-m|--minor <MINOR_VERSION>\n-r|--revision <REVISION_VERSION>\n-t|--start-year <START_YEAR>\n-s|--suffix <SUFFIX>"
# 参数校验
if [[ $# -eq 0 ]]; then
    echo -e $usage
    exit 1
fi
args=$(getopt -o "c:o:mv:sv:t:s:" $*)
if [ $? -ne 0 ]; then
    echo -e $usage
    exit 2
fi

tag=""
configuration=Debug
output=.
minorVersion=""
revisionVersion=""
startYear=$(date +%Y)
suffix=""

file=${@: -1}
for i; do
    case $i in
    -c | --configuration | -o | --output | -m | --minor | -r | --revision | -t | --start-year | -s | --suffix)
        tag=$i
        ;;
    *)
        case $tag in
        -c | --configuration)
            configuration=$i
            tag=""
            ;;
        -o | --output)
            output=$i
            tag=""
            ;;
        -m | --minor)
            minorVersion=$i
            tag=""
            ;;
        -r | --revision)
            revisionVersion=$i
            tag=""
            ;;
        -t | --start-year)
            startYear=$i
            tag=""
            ;;
        -s | --suffix)
            suffix=$i
            tag=""
            ;;
        esac
        ;;
    esac
done

if [[ ! -f "$file" ]]; then
    echo "文件不存在!"
    exit 3
fi

if [[ "${file##*.}"x != "csproj"x ]]; then
    echo "文件格式不支持!"
    exit 4
fi

if [[ "$minorVersion"x == ""x || "$revisionVersion"x == ""x ]]; then
    echo "未提供次要版本号或修订版本号!"
    exit 5
fi

currentYear=$(date +%Y)
offset=$(expr $currentYear - $startYear)
if [[ $offset -lt 0 ]]; then
    echo "起始年不可用!"
    exit 5
fi

# 计算时间戳版本号
start=$(date -j -f %Y-%m-%dT%H:%M:%S $startYear-01-01T00:00:00 +%s)
current=$(date +%s)
timestamp=$(expr $current - $start)
week=$(expr $timestamp / 604800)
if [[ "$suffix"x != ""x ]]; then
    suffix=-$suffix.$(expr $(expr $timestamp % 604800) / 60)
fi
buildVersion=$minorVersion.$revisionVersion.$week$suffix
sed -i "" "s/\(<BuildVersion>\)[^<]*\(<\)/\1$buildVersion\2/g" $file

# 构建
dotnet pack -c $configuration -o $output $file
