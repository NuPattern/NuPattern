ECHO OFF

REM Set current directory here
CD /d %~dp0

CD Processed\Unsigned\VSIXes

REM Add Signed VSIXes
CD 10.0
"%ProgramFiles%\7-Zip\7z.exe" a -tzip "NuPatternToolkitBuilder.vsix" "..\..\..\Signed\VSIXes\10.0\*.vsix" -mx9
IF %errorlevel% neq 0 GOTO :error

REM Add Signed VSIXes
CD ..
CD 11.0
"%ProgramFiles%\7-Zip\7z.exe" a -tzip "NuPatternToolkitBuilder.vsix" "..\..\..\Signed\VSIXes\11.0\*.vsix" -mx9
IF %errorlevel% neq 0 GOTO :error

CD ..\..\..\..

ECHO VSIXes Rebuilt Successfully!
COLOR 0A
PAUSE
COLOR
EXIT /b 0

:error
ECHO Failed Rebuilding of VSIXes! error #%errorlevel%
COLOR 04
PAUSE
COLOR