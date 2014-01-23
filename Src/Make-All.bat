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

ECHO Built MSI can be found in the 'Binaries' directory.
CD /d %~dp0
%SystemRoot%\explorer.exe "Binaries"
REM Warning: Explore.exe return and %errorlevel% of 1

ECHO NuPattern Built Successfully!
COLOR 0A
PAUSE
COLOR
EXIT /b 0

:error
ECHO Failed Building! error #%errorlevel%
COLOR 04
PAUSE
COLOR
EXIT /b %errorlevel%