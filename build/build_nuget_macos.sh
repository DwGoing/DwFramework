#!/bin/bash
usage="Usage:\n-c|--configuration <CONFIGURATION>\n-o|--output <OUTPUT_DIRECTORY>\n-v|--buildversion <BUILD_VERSION>\n-s|--suffix <VERSION_SUFFIX>"
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
buildVersion=""
reversion=""
suffix=""
file=${@: -1}
for i; do
    case $i in
    -c | --configuration | -o | --output | -v | --buildversion | -s | --suffix)
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
        -v | --buildversion)
            buildVersion=$i
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

if [ ! -f "$file" ]; then
    echo "文件不存在!"
    exit 3
fi

if [ "${file##*.}"x != "csproj"x ]; then
    echo "文件格式不支持!"
    exit 4
fi

if [ "$buildVersion"x == ""x ]; then
    echo "未提供BuildVersion!"
    exit 5
fi
sed -i "" "s/\(<BuildVersion>\)[^<]*\(<\)/\1$buildVersion\2/g" $file

# 计算时间戳版本号
Y=$(date +%Y)
start=$(date -j -f %Y-%m-%dT%H:%M:%S ${Y}-01-01T00:00:00 +%s)
current=$(date +%s)
reversion=$(expr $(expr $current - $start) / 60)
sed -i "" "s/\(<Reversion>\)[^<]*\(<\)/\1$reversion\2/g" $file

# 版本后缀
if [ "$suffix"x != ""x ]; then
    suffix="-"$suffix
fi
sed -i "" "s/\(<VersionSuffix>\)[^<]*\(<\)/\1$suffix\2/g" $file

# 构建
dotnet pack -c $configuration -o $output $file
