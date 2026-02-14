@echo off
setlocal

:: ============================================================
:: Church Attendance - Setup Script
:: Run this as Administrator on the church Windows machine.
:: It installs the app, generates HTTPS certs, and creates
:: a startup shortcut so the app runs automatically on login.
:: ============================================================

:: Check for admin privileges
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo This script must be run as Administrator.
    echo Right-click setup.bat and select "Run as administrator".
    pause
    exit /b 1
)

set INSTALL_DIR=C:\ChurchAttendance
set CERT_DIR=%INSTALL_DIR%\certs
set DATA_DIR=%USERPROFILE%\.church-attendance\data
set STARTUP_DIR=%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup

echo.
echo === Church Attendance Setup ===
echo.
echo Install directory : %INSTALL_DIR%
echo Data directory    : %DATA_DIR%
echo.

:: 1. Copy app files to install directory
echo [1/5] Copying app files to %INSTALL_DIR%...
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
xcopy /E /Y /I "%~dp0*" "%INSTALL_DIR%" >nul
if %errorlevel% neq 0 (
    echo ERROR: Failed to copy files.
    pause
    exit /b 1
)
echo       Done.

:: 2. Create data directory and seed data if empty
echo [2/5] Setting up data directory at %DATA_DIR%...
if not exist "%DATA_DIR%" mkdir "%DATA_DIR%"
if exist "%~dp0data\*.json" if not exist "%DATA_DIR%\members.json" (
    xcopy /Y "%~dp0data\*.json" "%DATA_DIR%\" >nul
    echo       Copied seed data.
)
if exist "%DATA_DIR%\members.json" echo       Data already present.
echo       Done.

:: 3. Generate self-signed HTTPS certificate
echo [3/5] Generating HTTPS certificate...
if not exist "%CERT_DIR%" mkdir "%CERT_DIR%"

powershell -ExecutionPolicy Bypass -Command ^
  "$hostname = $env:COMPUTERNAME;" ^
  "$cert = New-SelfSignedCertificate" ^
    " -DnsName 'localhost', '127.0.0.1', '::1', $hostname, \"$hostname.local\", '*.local'" ^
    " -CertStoreLocation 'Cert:\LocalMachine\My'" ^
    " -FriendlyName 'Church Attendance HTTPS'" ^
    " -NotAfter (Get-Date).AddYears(10);" ^
  "$pfxBytes = $cert.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Pfx);" ^
  "[System.IO.File]::WriteAllBytes('%CERT_DIR%\certificate.pfx', $pfxBytes);" ^
  "Write-Host \"       Certificate created for: localhost, $hostname, $hostname.local\";" ^
  "Write-Host \"       Thumbprint: $($cert.Thumbprint)\";"

if %errorlevel% neq 0 (
    echo ERROR: Failed to generate certificate.
    pause
    exit /b 1
)

:: 4. Trust the certificate (add to Trusted Root so browsers don't warn)
echo [4/5] Trusting certificate on this machine...
powershell -ExecutionPolicy Bypass -Command ^
  "$pfx = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2('%CERT_DIR%\certificate.pfx');" ^
  "$store = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root', 'LocalMachine');" ^
  "$store.Open('ReadWrite');" ^
  "$store.Add($pfx);" ^
  "$store.Close();" ^
  "Write-Host '       Certificate added to Trusted Root store.';"

if %errorlevel% neq 0 (
    echo WARNING: Could not trust certificate. Browsers may show a warning.
)

:: 5. Create startup shortcut
echo [5/5] Creating startup shortcut...
powershell -ExecutionPolicy Bypass -Command ^
  "$ws = New-Object -ComObject WScript.Shell;" ^
  "$shortcut = $ws.CreateShortcut('%STARTUP_DIR%\ChurchAttendance.lnk');" ^
  "$shortcut.TargetPath = '%INSTALL_DIR%\ChurchAttendance.exe';" ^
  "$shortcut.WorkingDirectory = '%INSTALL_DIR%';" ^
  "$shortcut.Description = 'Church Attendance App';" ^
  "$shortcut.Save();" ^
  "Write-Host '       Shortcut created at: %STARTUP_DIR%\ChurchAttendance.lnk';"

echo.
echo === Setup Complete ===
echo.
echo The app is installed at %INSTALL_DIR%
echo It will start automatically when you log in.
echo.
echo To start it now, run: %INSTALL_DIR%\ChurchAttendance.exe
echo Then open a browser to:
echo   http://localhost:5050       (HTTP)
echo   https://localhost:5051      (HTTPS)
echo   https://%COMPUTERNAME%:5051 (from other devices on the network)
echo.

:: Ask if user wants to allow through Windows Firewall
choice /C YN /M "Allow through Windows Firewall (needed for network access)"
if %errorlevel% equ 1 (
    netsh advfirewall firewall add rule name="Church Attendance HTTP" dir=in action=allow protocol=TCP localport=5050 >nul
    netsh advfirewall firewall add rule name="Church Attendance HTTPS" dir=in action=allow protocol=TCP localport=5051 >nul
    echo Firewall rules added.
)

echo.
choice /C YN /M "Start the app now"
if %errorlevel% equ 1 (
    start "" "%INSTALL_DIR%\ChurchAttendance.exe"
    timeout /t 2 >nul
    start "" "http://localhost:5050"
)

echo.
echo Done!
pause
