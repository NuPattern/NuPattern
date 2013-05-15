ECHO OFF

REM Set current directory here
CD /d %~dp0

REM Recreate directories
RMDIR /s /q Processed
MKDIR Processed
MKDIR Processed\Signed
MKDIR Processed\Unsigned
MKDIR Processed\Unsigned\Assemblies
MKDIR Processed\Unsigned\Assemblies\10.0
MKDIR Processed\Unsigned\Assemblies\11.0
MKDIR Processed\Unsigned\VSIXes
MKDIR Processed\Unsigned\VSIXes\10.0
MKDIR Processed\Unsigned\VSIXes\11.0

REM Copy Built VSIXes
XCOPY /s /f /r ..\Binaries\\*.vsix Processed\Unsigned\VSIXes
IF %errorlevel% neq 0 GOTO :error


ECHO VSIXes Harvested Successfully!
COLOR 0A
PAUSE
COLOR
EXIT /b 0

:error
ECHO Failed Havesting! error #%errorlevel%
COLOR 04
PAUSE
COLOR