ECHO OFF

REM Set current directory here
CD /d %~dp0

CD Processed\Unsigned\VSIXes

REM Add Signed assemblies
CD 10.0
for /f "delims=" %%a in ('dir /s/b *.vsix') do ("%ProgramFiles%\7-Zip\7z.exe" a -tzip "%%a" "..\..\..\Signed\Assemblies\10.0\%%~na\NuPattern.*.dll" -mx9)
IF %errorlevel% neq 0 GOTO :error

REM Add Signed assemblies
CD ..
CD 11.0
for /f "delims=" %%a in ('dir /s/b *.vsix') do ("%ProgramFiles%\7-Zip\7z.exe" a -tzip "%%a" "..\..\..\Signed\Assemblies\11.0\%%~na\NuPattern.*.dll" -mx9)
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