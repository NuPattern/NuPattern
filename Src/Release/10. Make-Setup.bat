ECHO OFF

SET vsname=%1
SET vsver="11.0"

SET progfiles=%ProgramFiles%
IF EXIST "%ProgramFiles(x86)%" SET progfiles=%ProgramFiles(x86)%

REM Setup the Visual Studio Build Environment
CALL "%progfiles%\Microsoft Visual Studio %vsver%\VC\vcvarsall.bat" x86
IF %errorlevel% neq 0 GOTO :error

CD /d %~dp0
CD ..

REM Build MSI Installer
CD Authoring
msbuild.exe Authoring.Setup.vs2012.sln /t:Rebuild /p:Configuration=Debug-VS2012
IF %errorlevel% neq 0 GOTO :error

REM Copy MSI back to Unsigned
CD /d %~dp0
XCOPY /s /f /r /y ..\Binaries\*.msi Processed\Unsigned


ECHO Built MSI can be found in the 'UnSigned' directory.
CD /d %~dp0
%SystemRoot%\explorer.exe "Processed\Unsigned"
REM Warning: Explore.exe return and %errorlevel% of 1

ECHO NuPattern Installer Built Successfully!
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