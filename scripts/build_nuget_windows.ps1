# 输入参数验证
param(
[int]$mv=$(throw "Parameter missing: -mv x as number"),
[int]$sv=$(throw "Parameter missing: -sv x as number"),
[string]$sx=$(throw "Parameter missing: -sx suffix"),
[string]$fp=$(throw "Parameter missing: -fp filePath"),
[string]$c=$(throw "Parameter missing: -c configuration"),
[string]$op=$(throw "Parameter missing: -op output"))

# 接收参数
$mainVersion=$mv
$subVersion=$sv
$suffix=$sx
$filePath=$fp
$configuration=$c
$output=$op

# 计算时间戳版本号
$start="2020-01-01 00:00:00"
$current=Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$timestamp=(New-TimeSpan $start -end $current).Ticks / 10000000
$week=[math]::truncate($timestamp / 604800)
$buildVersion= -join ($mainVersion,".",$subVersion,".",$week,"-",$suffix,".",[math]::truncate(($timestamp % 604800) / 60))

# 替换csproj
(Get-Content $filePath -encoding utf8) -replace("<BuildVersion>.*</BuildVersion>", -join ("<BuildVersion>",$buildVersion,"</BuildVersion>")) |Set-Content $filePath -encoding utf8

# 打包
dotnet pack --no-build --configuration $configuration --output $output


