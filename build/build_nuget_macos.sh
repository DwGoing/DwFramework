#!/bin/bash
CORE_PATH=$(cd "$(dirname "$0")" && pwd)/../src/DwFramework.Core/DwFramework.Core.csproj

usage="Usage:\n-c|--configuration <CONFIGURATION>\n-o|--output <OUTPUT_DIRECTORY>\n-m|--minor <MINOR_VERSION>\n-s|--suffix <SUFFIX>"
# 参数
if [[ $# -eq 0 ]]; then
    echo -e $usage
    exit 1
fi
args=$(getopt -o "c:o:mv:sv:t:s:" $*)
if [ $? -ne 0 ]; then
    echo -e $usage
    exit 2
fi
file=${@: -1}
if [[ "$file"x == ""x || "${file##*.}"x != "csproj"x ]]; then
    echo "缺少目标项目文件或者文件类型错误"
    exit 3
fi

#获取当前版本号
coreVersion=$(cat $CORE_PATH | grep -E '<BuildVersion>' | sed "s/<BuildVersion>//g" | sed "s/<\/BuildVersion>//g")
OLD_IFS="$IFS"
IFS="."
array=($coreVersion)
IFS="$OLD_IFS"
NET_VERSION=${array[0]}       # 和.NET版本保持一致
FRAMEWORK_VERSION=${array[1]} # 和DwFramework.Core保持一致
if [[ "$coreVersion"x == ""x || "$NET_VERSION"x == ""x || "$FRAMEWORK_VERSION"x == ""x ]]; then
    echo "DwFramework.Core项目文件版本号错误"
    exit 4
fi
currentVersion=$(cat $file | grep -E '<BuildVersion>' | sed "s/<BuildVersion>//g" | sed "s/<\/BuildVersion>//g")
if [[ "$currentVersion"x == ""x ]]; then
    echo "无法获取项目文件版本号信息"
    exit 5
fi
MINOR_VERSION=${array[2]}
REVISION_VERSION=${array[3]}

tag=""
configuration=Debug
output=.
startYear=$(date +%Y)
suffix=""

for i; do
    case $i in
    -c | --configuration | -o | --output | -m | --minor | -s | --suffix)
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
            MINOR_VERSION=$i
            tag=""
            ;;
        -s | --suffix)
            suffix=.$i
            tag=""
            ;;
        esac
        ;;
    esac
done

if [[ "$MINOR_VERSION"x == ""x ]]; then
    MINOR_VERSION=0
fi
if [[ "$REVISION_VERSION"x == ""x ]]; then
    REVISION_VERSION=0
else
    REVISION_VERSION=$(expr $REVISION_VERSION + 1)
fi

# 计算时间戳版本号
start=$(date -j -f %Y-%m-%dT%H:%M:%S 1970-01-01T00:00:00 +%s)
current=$(date +%s)
timestamp=$(expr $current - $start)
buildVersion=$NET_VERSION.$FRAMEWORK_VERSION.$MINOR_VERSION.$REVISION_VERSION-$timestamp$suffix
sed -i "" "s/\(<BuildVersion>\)[^<]*\(<\)/\1$buildVersion\2/g" $file

# 构建
dotnet pack -c $configuration -o $output $file
