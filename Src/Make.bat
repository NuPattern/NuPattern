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

ECHO:
CALL SetColor.bat 02 "-- Building Runtime (%vsname%)"
ECHO(
ECHO:

REM Build Runtime
CD Runtime
msbuild.exe Runtime.vs%vsname%.sln /t:Rebuild /p:Configuration=Debug-VS%vsname%;ExecuteTests=true /verbosity:minimal
IF %errorlevel% neq 0 GOTO :error

ECHO:
CALL ..\SetColor.bat 0A "---- Runtime (%vsname%) Built Successfully!"
ECHO(
ECHO:
CALL ..\SetColor.bat 02 "-- Building Authoring (%vsname%)"
ECHO(
ECHO:

REM Build Authoring
CD ..\Authoring
msbuild.exe Authoring.vs%vsname%.sln /t:Rebuild /p:Configuration=Debug-VS%vsname%;ExecuteTests=true /verbosity:minimal
IF %errorlevel% neq 0 GOTO :error

ECHO:
CALL ..\SetColor.bat 0A "---- Authoring (%vsname%) Built Successfully!"
ECHO(
ECHO:


CD /d %~dp0
ECHO:
CALL SetColor.bat 0F "INFO -- Built VSIXes can be found in the 'Binaries-%vsver%' directory."
ECHO(
IF NOT "%silent%"=="q" %SystemRoot%\explorer.exe "Binaries\%vsver%"
REM Warning: Explore.exe return and %errorlevel% of 1

ECHO:
CALL SetColor.bat 0A "---- NuPattern (%vsname%) Built Successfully!"
ECHO(
ECHO:
IF NOT "%silent%"=="q" PAUSE
EXIT /b 0

:error
CD /d %~dp0
ECHO:
ECHO:
CALL SetColor.bat 04 "**** Failed Building! error #%errorlevel% ****"
ECHO(
PAUSE
EXIT /b %errorlevel%