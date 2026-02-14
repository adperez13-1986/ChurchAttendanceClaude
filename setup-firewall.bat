@echo off
REM Church Attendance App - Firewall Setup Script
REM Run this as Administrator on the church PC

echo ========================================
echo Church Attendance - Firewall Setup
echo ========================================
echo.
echo This script will configure Windows Firewall to allow
echo incoming connections on port 5050 (for LAN access).
echo.
echo IMPORTANT: You must run this as Administrator!
echo.
pause

REM Check for admin privileges
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: This script must be run as Administrator!
    echo.
    echo Right-click this file and select "Run as administrator"
    echo.
    pause
    exit /b 1
)

echo.
echo Adding firewall rule...

REM Remove existing rule if it exists (in case of re-install)
netsh advfirewall firewall delete rule name="Church Attendance App" >nul 2>&1

REM Add new firewall rule
netsh advfirewall firewall add rule name="Church Attendance App" dir=in action=allow protocol=TCP localport=5050

if errorlevel 1 (
    echo ERROR: Failed to add firewall rule
    pause
    exit /b 1
)

echo.
echo ========================================
echo Firewall Setup Complete!
echo ========================================
echo.
echo Port 5050 is now allowed through Windows Firewall.
echo The app will be accessible from phones and tablets on the same network.
echo.
echo To find this PC's IP address, run: ipconfig
echo Look for "IPv4 Address" under your active network adapter.
echo.
pause
