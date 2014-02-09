ECHO OFF

REM Build VS2010 Versions
CD /d %~dp0
CALL Make.bat 2010 q
IF %errorlevel% neq 0 GOTO :error

REM Build VS2012 Versions
CD /d %~dp0 
CALL Make.bat 2012 q
IF %errorlevel% neq 0 GOTO :error

REM Build VS2013 Versions
CD /d %~dp0 
CALL Make.bat 2013 q
IF %errorlevel% neq 0 GOTO :error

REM Build MSI Installer
CD Authoring
msbuild.exe Authoring.Setup.vs2013.sln /t:Rebuild /p:Configuration=Debug-VS2013
IF %errorlevel% neq 0 GOTO :error

CD /d %~dp0
ECHO:
CALL SetColor.bat 0F "INFO -- Built MSI can be found in the 'Binaries' directory"
ECHO(
%SystemRoot%\explorer.exe "Binaries"
REM Warning: Explore.exe return and %errorlevel% of 1

CD /d %~dp0
ECHO:
ECHO:
CALL SetColor.bat 0A "---- NuPattern (All Versions) Built Successfully!"
ECHO(
PAUSE
EXIT /b 0

:error
CD /d %~dp0
ECHO:
ECHO:
CALL SetColor.bat 04 "**** Failed Building! error #%errorlevel% ****"
ECHO(
PAUSE
EXIT /b %errorlevel%