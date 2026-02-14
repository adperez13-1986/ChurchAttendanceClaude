@echo off
tasklist /FI "IMAGENAME eq ChurchAttendance.exe" 2>nul | find /I "ChurchAttendance.exe" >nul
if %errorlevel% equ 0 (
    echo Church Attendance is already running.
) else (
    start "" "C:\ChurchAttendance\ChurchAttendance.exe"
    echo Church Attendance started.
)
pause
