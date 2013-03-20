echo ON
SET platform=%1
SET vsver=%2
SET vsname=%3

REM Setup the Visual Studio Build Environment
REM
call "C:\Program Files (x86)\Microsoft Visual Studio %vsver%\VC\vcvarsall.bat" %platform%
rem if %errorlevel% neq 0 goto :error

REM Build Runtime
cd Runtime
msbuild.exe Runtime.vs%vsname%.sln /t:Rebuild /p:Configuration=Debug-VS%vsname%
rem if %errorlevel% neq 0 goto :error

REM Build Authoring
cd ..\Authoring
msbuild.exe Authoring.vs%vsname%.sln /t:Rebuild /p:Configuration=Debug-VS%vsname%
rem if %errorlevel% neq 0 goto :error


ECHO Built VSIXes can be found in the 'Binaries\%vsver%' directory.
cd /d %~dp0
%SystemRoot%\explorer.exe "Binaries\%vsver%"

ECHO NuPattern Built Successfully!
color 0A
pause
goto :EOF

:error
ECHO Failed Building! error #%errorlevel%
color 04
pause
exit %errorlevel%