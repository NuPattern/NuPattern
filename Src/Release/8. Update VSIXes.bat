ECHO OFF

REM Set current directory here
CD /d %~dp0


REM Copy Built VSIXes
XCOPY /s /f /r /y Processed\Signed\\*.vsix ..\Binaries
IF %errorlevel% neq 0 GOTO :error


ECHO VSIXes Updated Successfully!
COLOR 0A
PAUSE
COLOR
EXIT /b 0

:error
ECHO Failed Update! error #%errorlevel%
COLOR 04
PAUSE
COLOR