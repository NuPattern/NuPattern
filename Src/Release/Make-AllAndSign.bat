ECHO OFF

ECHO Credentials to access the Outercurve signing service?
SET /P signusername= Username :
SET /P signuserpassword=Password :

CD ..\

REM Build VS2010 Versions
CALL Make.bat 2010 q
IF %errorlevel% neq 0 GOTO :error

REM Build VS2012 Versions
CALL Make.bat 2012 q
IF %errorlevel% neq 0 GOTO :error

REM Build VS2013 Versions
CALL Make.bat 2013 q
IF %errorlevel% neq 0 GOTO :error

REM Sign the Binaries
msbuild.exe Make-AllandSign.proj /t:SignAllBinaries /p:SignUserName=%signusername%;SignUserPassword=%signuserpassword%
IF %errorlevel% neq 0 GOTO :error

ECHO Built and Signed Successfully!
COLOR 0A
PAUSE
COLOR
EXIT /b 0

:error
ECHO Failed Build and Signing! error #%errorlevel%
COLOR 04
PAUSE
COLOR