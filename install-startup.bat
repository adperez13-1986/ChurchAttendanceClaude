@echo off
REM Church Attendance App - Install to Windows Startup
REM Run this on the church PC to make the app start automatically

echo ========================================
echo Church Attendance - Startup Install
echo ========================================
echo.
echo This script will configure the app to start automatically
echo when Windows boots (or when you log in).
echo.
pause

REM Get the current directory where this script is running
set INSTALL_DIR=%~dp0

REM Check if ChurchAttendance.exe exists
if not exist "%INSTALL_DIR%ChurchAttendance.exe" (
    echo ERROR: ChurchAttendance.exe not found in %INSTALL_DIR%
    echo.
    echo Make sure you're running this script from the 'publish' folder
    echo where ChurchAttendance.exe is located.
    echo.
    pause
    exit /b 1
)

echo.
echo Installing to startup folder...

REM Create a shortcut in the Startup folder
set STARTUP_FOLDER=%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup
set SHORTCUT_PATH=%STARTUP_FOLDER%\Church Attendance.lnk

REM Use PowerShell to create the shortcut
powershell -Command "$WS = New-Object -ComObject WScript.Shell; $SC = $WS.CreateShortcut('%SHORTCUT_PATH%'); $SC.TargetPath = '%INSTALL_DIR%ChurchAttendance.exe'; $SC.WorkingDirectory = '%INSTALL_DIR%'; $SC.Description = 'Church Attendance App'; $SC.Save()"

if errorlevel 1 (
    echo ERROR: Failed to create startup shortcut
    pause
    exit /b 1
)

echo.
echo ========================================
echo Startup Install Complete!
echo ========================================
echo.
echo The Church Attendance app will now start automatically when Windows boots.
echo.
echo Location: %INSTALL_DIR%
echo Shortcut: %SHORTCUT_PATH%
echo.
echo You can:
echo - Start it now by double-clicking ChurchAttendance.exe
echo - It will auto-start on next reboot
echo.
echo Access the app at: http://localhost:5050
echo.
pause
