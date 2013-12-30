ECHO OFF

SET vsname=%1
IF %1==2010 SET vsver="10.0"
IF %1==2012 SET vsver="11.0"
IF %1==2013 SET vsver="12.0"
SET silent=%2

SET progfiles=%ProgramFiles%
IF EXIST "%ProgramFiles(x86)%" SET progfiles=%ProgramFiles(x86)%


REM Setup the Visual Studio Build Environment
CALL "%progfiles%\Microsoft Visual Studio %vsver%\VC\vcvarsall.bat" x86
IF %errorlevel% neq 0 GOTO :error

REM Build Runtime
CD Runtime
msbuild.exe Runtime.vs%vsname%.sln /t:Rebuild /p:Configuration=Debug-VS%vsname%
IF %errorlevel% neq 0 GOTO :error

REM Build Authoring
CD ..\Authoring
msbuild.exe Authoring.vs%vsname%.sln /t:Rebuild /p:Configuration=Debug-VS%vsname%
IF %errorlevel% neq 0 GOTO :error


ECHO Built VSIXes can be found in the 'Binaries\%vsver%' directory.
CD /d %~dp0
IF NOT "%silent%"=="q" %SystemRoot%\explorer.exe "Binaries\%vsver%"
REM Warning: Explore.exe return and %errorlevel% of 1

ECHO NuPattern Built Successfully!
COLOR 0A
IF NOT "%silent%"=="q" PAUSE
COLOR
EXIT /b 0

:error
ECHO Failed Building! error #%errorlevel%
COLOR 04
PAUSE
COLOR
EXIT /b %errorlevel%