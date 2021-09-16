#!/bin/bash
CORE_PATH=$(cd "$(dirname "$0")" && pwd)/../src/DwFramework.Core/DwFramework.Core.csproj

usage="Usage:\n-c|--configuration <CONFIGURATION>\n-o|--output <OUTPUT_DIRECTORY>\n-m|--minor <MINOR_VERSION>\n-s|--suffix <SUFFIX>"
# 参数验证
if [[ $# -eq 0 ]]; then
    echo -e $usage
    exit 1
fi
args=$(getopt -o "c:o:mv:sv:t:s:" $*)
if [ $? -ne 0 ]; then
    echo -e $usage
    exit 2
fi
FILE=${@: -1}
if [[ "$FILE"x == ""x || "${FILE##*.}"x != "csproj"x ]]; then
    echo "缺少目标项目文件或者文件类型错误"
    exit 3
fi

# 生成版本号
coreVersion=$(cat $CORE_PATH | grep -E '<Version>' | sed "s/<Version>//g" | sed "s/<\/Version>//g" | sed 's/^[ \t]*//g')
coreVersion=$(awk 'BEGIN {split('"\"$coreVersion\""',a,"-"); print a[1]}')
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
currentVersion=$(cat $FILE | grep -E '<Version>' | sed "s/<Version>//g" | sed "s/<\/Version>//g" | sed 's/^[ \t]*//g')
currentVersion=$(awk 'BEGIN {split('"\"$currentVersion\""',a,"-"); print a[1]}')
if [[ "$currentVersion"x == ""x ]]; then
    echo "无法获取项目文件版本号信息"
    exit 5
fi
OLD_IFS="$IFS"
IFS="."
array=($currentVersion)
IFS="$OLD_IFS"
CURRENT_FRAMEWORK_VERSION=${array[1]}
MINOR_VERSION=${array[2]}
REVISION_VERSION=${array[3]}

CONFIGURATION=Debug
OUTPUT=.
SUFFIX=""

tag=""
for i; do
    case $i in
    -c | --configuration | -o | --output | -m | --minor | -s | --suffix)
        tag=$i
        ;;
    *)
        case $tag in
        -c | --configuration)
            CONFIGURATION=$i
            tag=""
            ;;
        -o | --output)
            OUTPUT=$i
            tag=""
            ;;
        -m | --minor)
            if [[ $MINOR_VERSION != $i ]]; then
                REVISION_VERSION=""
            fi
            if [[ $(echo $i | sed "s/[0-9]//g")x != ""x ]]; then
                MINOR_VERSION=""
            else
                MINOR_VERSION=$i
            fi
            tag=""
            ;;
        -s | --suffix)
            if [[ "$i"x != ""x ]]; then
                start=$(date -j -f %Y-%m-%dT%H:%M:%S 1970-01-01T00:00:00 +%s)
                current=$(date +%s)
                timestamp=$(expr $current - $start)
                SUFFIX=-$timestamp.$i
            fi
            tag=""
            ;;
        esac
        ;;
    esac
done

if [[ $CURRENT_FRAMEWORK_VERSION != $FRAMEWORK_VERSION ]]; then
    MINOR_VERSION=""
    REVISION_VERSION=""
fi
if [[ "$MINOR_VERSION"x == ""x ]]; then
    MINOR_VERSION=0
fi
if [[ "$REVISION_VERSION"x == ""x ]]; then
    REVISION_VERSION=0
else
    REVISION_VERSION=$(expr $REVISION_VERSION + 1)
fi

version=$NET_VERSION.$FRAMEWORK_VERSION.$MINOR_VERSION.$REVISION_VERSION$SUFFIX
sed -i "" "s/\(<Version>\)[^<]*\(<\)/\1$version\2/g" $FILE

# 构建
dotnet pack -c $CONFIGURATION -o $OUTPUT $FILE
