:: Prepare nuget package

:: --- NFireLogger ---

mkdir nfirelogger\lib\net40\
mkdir nfirelogger\content\

copy ..\NFireLogger\bin\Release\NFireLogger.dll nfirelogger\lib\net40\
copy web.config.transform nfirelogger\content\

copy NFireLogger.nuspec nfirelogger\

nuget pack nfirelogger\NFireLogger.nuspec

:: --- NFireLogger.Log4net ---

mkdir nfirelogger-log4net\lib\net40\

copy ..\NFireLogger.Log4net\bin\Release\NFireLogger.Log4net.dll nfirelogger-log4net\lib\net40\

copy NFireLogger.Log4net.nuspec nfirelogger-log4net\

nuget pack nfirelogger-log4net\NFireLogger.Log4net.nuspec


