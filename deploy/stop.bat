@echo off
taskkill /IM ChurchAttendance.exe /F >nul 2>&1
if %errorlevel% equ 0 (
    echo Church Attendance stopped.
) else (
    echo Church Attendance is not running.
)
pause
