echo off
:SetColor Color String
::
:: Prints String in color specified by Color.
::  i.e. call SetColor f4 "Hello"
::
::   Color should be 2 hex digits
::     The 1st digit specifies the background
::     The 2nd digit specifies the foreground
::     See COLOR /? for more help
::
::   String is the text to print. All quotes will be stripped.
::     The string cannot contain any of the following: * ? < > | : \ /
::     Also, any trailing . or <space> will be stripped.
::
::   The string is printed to the screen without issuing a <newline>,
::   so multiple colors can appear on one line. To terminate the line
::   without printing anything, use the ECHO( command.
::
setlocal
pushd %temp%
for /F "tokens=1 delims=#" %%a in ('"prompt #$H#$E# & echo on & for %%b in (1) do rem"') do (
  <nul set/p"=%%a" >"%~2"
)
findstr /v /a:%1 /R "^$" "%~2" nul
del "%~2" > nul 2>&1
popd
exit /b