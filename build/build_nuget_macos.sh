#!/bin/bash
usage="Usage:\n-c|--configuration <CONFIGURATION>\n-o|--output <OUTPUT_DIRECTORY>\n-s|--suffix <VERSION_SUFFIX>\n-v|--version <VERSION>"
# 参数校验
if [ $# -eq 0 ]; then
    echo -e usage
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
version=""
reversion=""
suffix=""
startYear=2018
file=${@: -1}
for i; do
    case $i in
    -c | --configuration | -o | --output | -s | --suffix | -v | --buildversion)
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
        -s | --suffix)
            suffix=$i
            tag=""
            ;;
        -v | --version)
            version=$i
            tag=""
            ;;
        esac
        ;;
    esac
done

if [ ! -f "$file" ]; then
    echo "文件不存在!"
    exit 3
fi

if [ "${file##*.}"x != "csproj"x ]; then
    echo "文件格式不支持!"
    exit 4
fi

if [ "$version"x == ""x ]; then
    echo "未提供Version!"
    exit 5
fi

currentYear=$(date +%Y)
if [[ $version -lt 0 || $(expr $currentYear - $startYear) -lt $version ]]; then
    echo "Version不可用!"
    exit 5
fi

# 计算时间戳版本号
start=$(date -j -f %Y-%m-%dT%H:%M:%S $(expr $startYear + $version)-01-01T00:00:00 +%s)
current=$(date +%s)
timestamp=$(expr $current - $start)
week=$(expr $timestamp / 604800)
buildVersion=$version.$week.$(expr $(expr $timestamp % 604800) / 60)
sed -i "" "s/\(<BuildVersion>\)[^<]*\(<\)/\1$buildVersion\2/g" $file

# 版本后缀
if [ "$suffix"x != ""x ]; then
    suffix="-"$suffix
fi
sed -i "" "s/\(<VersionSuffix>\)[^<]*\(<\)/\1$suffix\2/g" $file

# 构建
dotnet pack -c $configuration -o $output $file
