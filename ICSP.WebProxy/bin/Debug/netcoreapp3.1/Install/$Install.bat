@echo off
cls

echo ----------------------------------------------------------------------------------------------------
echo Installing ICSP.WebProxy Service...
echo Path=%~dp0
echo ----------------------------------------------------------------------------------------------------

sc create ICSP.WebProxy binPath= "%~dp0..\ICSP.WebProxy.exe" start=auto
sc description ICSP.WebProxy "Schnittstelle fuer HTML-WebSockets und AMX ICSP-Protokoll"

echo ----------------------------------------------------------------------------------------------------
echo Apply Security Settings for ICSP.WebProxy...
echo ----------------------------------------------------------------------------------------------------

rem reg query "hklm\system\controlset001\control\nls\language" /v Installlanguage
rem HKEY_LOCAL_MACHINE\system\controlset001\control\nls\language
rem Installlanguage    REG_SZ    0407
rem 0407 --> German
rem 0409 --> English

for /f "tokens=3" %%a IN ('reg query "hklm\system\controlset001\control\nls\language" /v Installlanguage ^| find "Installlanguage"') do set Installlanguage=%%a
rem echo Installlanguage: %Installlanguage%

if %Installlanguage%==0407 "%~dp0\subinacl.exe" /service ICSP.WebProxy /grant=Jeder=PTO >nul
if %Installlanguage%==0409 "%~dp0\subinacl.exe" /service ICSP.WebProxy /grant=Users=PTO >nul

echo ----------------------------------------------------------------------------------------------------
echo Apply Security Settings for Remote Access ...
echo ----------------------------------------------------------------------------------------------------

rem Important:
rem If a remote scope is used, the following registry value is required on the server!
rem HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\system\LocalAccountTokenFilterPolicy = 1

reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v LocalAccountTokenFilterPolicy /t REG_DWORD /d 1 /f
echo ----------------------------------------------------------------------------------------------------
echo Done
echo ----------------------------------------------------------------------------------------------------
pause
