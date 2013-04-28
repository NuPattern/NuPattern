ECHO OFF

REM Set current directory here
CD /d %~dp0

REM Recreate directories
RMDIR /s /q Processed
MKDIR Processed
MKDIR Processed\Unsigned
MKDIR Processed\Signed

REM Copy Built VSIXes
XCOPY /s /f /r ..\Binaries\\*.vsix Processed\Unsigned
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