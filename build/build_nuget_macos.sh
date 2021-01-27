#!/bin/bash
usage="Usage:\n-c|--configuration <CONFIGURATION>\n-o|--output <OUTPUT_DIRECTORY>\n-mv|--main-version <MAIN_VERSION>\n-sv|--sub-version <SUB_VERSION>\n|-t|--start-year <START_YEAR>\n-s|--suffix <SUFFIX>"
# 参数校验
if [[ $# -eq 0 ]]; then
    echo -e $usage
    exit 1
fi
args=$(getopt cosv $*)
if [ $? -ne 0 ]; then
    echo -e usage
    exit 2
fi

tag=""
configuration=Debug
output=.
mainVersion=""
subVersion=""
startYear=2020
suffix=rc

file=${@: -1}
for i; do
    case $i in
    -c | --configuration | -o | --output | -mv | --main-version | -sv | --sub-version | -t | --start-year | -s | --suffix)
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
        -mv | --mian-version)
            mainVersion=$i
            tag=""
            ;;
        -sv | --sub-version)
            subVersion=$i
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

if [[ "$mainVersion"x == ""x || "$subVersion"x == ""x ]]; then
    echo "未提供主版本号或子版本号!"
    exit 5
fi

currentYear=$(date +%Y)
offset=$(expr $currentYear - $startYear)
if [[ $offset -lt 0 ]]; then
    echo "起始年不可用!"
    exit 5
fi

if [[ "$suffix"x == ""x ]]; then
    echo "后缀不可用!"
    exit 5
fi

# 计算时间戳版本号
start=$(date -j -f %Y-%m-%dT%H:%M:%S $startYear-01-01T00:00:00 +%s)
current=$(date +%s)
timestamp=$(expr $current - $start)
week=$(expr $timestamp / 604800)
buildVersion=$mainVersion.$subVersion.$week-$suffix.$(expr $(expr $timestamp % 604800) / 60)
sed -i "" "s/\(<BuildVersion>\)[^<]*\(<\)/\1$buildVersion\2/g" $file

# 构建
dotnet pack -c $configuration -o $output $file
