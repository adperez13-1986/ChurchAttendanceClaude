@echo off
REM Church Attendance App - Deployment Script
REM This script publishes the app as a self-contained Windows executable

echo ========================================
echo Church Attendance - Deployment Script
echo ========================================
echo.

REM Clean previous builds
echo [1/3] Cleaning previous builds...
dotnet clean -c Release
if errorlevel 1 (
    echo ERROR: Clean failed
    pause
    exit /b 1
)

REM Publish as self-contained single-file exe for Windows x64
echo.
echo [2/3] Publishing application...
echo This may take a few minutes...
dotnet publish ChurchAttendance/ChurchAttendance.fsproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish

if errorlevel 1 (
    echo ERROR: Publish failed
    pause
    exit /b 1
)

REM Copy data folder to publish directory
echo.
echo [3/3] Copying data files...
xcopy /E /I /Y ChurchAttendance\data publish\data

echo.
echo ========================================
echo Deployment Complete!
echo ========================================
echo.
echo Published files are in: %CD%\publish
echo.
echo NEXT STEPS:
echo 1. Copy the entire 'publish' folder to the church PC
echo 2. Run 'setup-firewall.bat' (as Administrator) on the church PC
echo 3. Run 'install-startup.bat' on the church PC to auto-start on boot
echo.
echo The app will be accessible at:
echo   - On the church PC: http://localhost:5050
echo   - On phones/tablets: http://[PC-IP-ADDRESS]:5050
echo.
pause
