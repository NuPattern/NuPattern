ECHO OFF

REM Set current directory here
CD /d %~dp0

CD Processed\Unsigned\VSIXes

REM Extract all assemblies
CD 10.0
for /f "delims=" %%a in ('dir /s/b *.vsix') do ("%ProgramFiles%\7-Zip\7z.exe" e -tzip "%%a" NuPattern.*.dll -o..\..\Assemblies\10.0\*)
IF %errorlevel% neq 0 GOTO :error

REM Extract all assemblies
CD ..
CD 11.0
for /f "delims=" %%a in ('dir /s/b *.vsix') do ("%ProgramFiles%\7-Zip\7z.exe" e -tzip "%%a" NuPattern.*.dll -o..\..\Assemblies\11.0\*)
IF %errorlevel% neq 0 GOTO :error

CD ..\..\..\..

ECHO Assemblies Extracted Successfully!
COLOR 0A
PAUSE
COLOR
EXIT /b 0

:error
ECHO Failed Extraction! error #%errorlevel%
COLOR 04
PAUSE
COLOR