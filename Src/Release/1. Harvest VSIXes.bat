ECHO OFF

REM Set current directory here
CD /d %~dp0

REM Recreate directories
RMDIR /s /q Processed
MKDIR Processed
MKDIR Processed\Signed
MKDIR Processed\Unsigned
MKDIR Processed\Unsigned\Assemblies
MKDIR Processed\Unsigned\Assemblies\10.0
MKDIR Processed\Unsigned\Assemblies\11.0
MKDIR Processed\Unsigned\VSIXes
MKDIR Processed\Unsigned\VSIXes\10.0
MKDIR Processed\Unsigned\VSIXes\11.0

REM Copy Built VSIXes
XCOPY /s /f /r ..\Binaries\\*.vsix Processed\Unsigned\VSIXes
IF %errorlevel% neq 0 GOTO :error


REM Re-compress all VSIXes
CD Processed\Unsigned\VSIXes

CD 10.0

for /f "delims=" %%a in ('dir /s/b *.vsix') do (
"%ProgramFiles%\7-Zip\7z.exe" x -tzip "%%a" -o%%~na
IF %errorlevel% neq 0 GOTO :error
DEL %%a
"%ProgramFiles%\7-Zip\7z.exe" a -tzip %%~na.vsix ".\%%~na\*" -mx9
IF %errorlevel% neq 0 GOTO :error
RMDIR /s /q %%~na
IF %errorlevel% neq 0 GOTO :error
)
CD..

CD 11.0

for /f "delims=" %%a in ('dir /s/b *.vsix') do (
"%ProgramFiles%\7-Zip\7z.exe" x -tzip "%%a" -o%%~na
IF %errorlevel% neq 0 GOTO :error
DEL %%a
"%ProgramFiles%\7-Zip\7z.exe" a -tzip %%~na.vsix ".\%%~na\*" -mx9
IF %errorlevel% neq 0 GOTO :error
RMDIR /s /q %%~na
IF %errorlevel% neq 0 GOTO :error
)
CD..

REM CD ..\..\..\


ECHO VSIXes Harvested Successfully!
COLOR 0A
PAUSE
COLOR
EXIT /b 0

:error
ECHO Failed Havesting! error #%errorlevel%
COLOR 04
PAUSE
COLOR