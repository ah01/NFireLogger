:: Prepare nuget package

mkdir temp\lib\net40\
mkdir temp\content\

copy ..\NFireLogger\bin\Release\NFireLogger.dll temp\lib\net40\
copy web.config.transform temp\content\

copy NFireLogger.nuspec temp\

nuget pack temp\NFireLogger.nuspec


