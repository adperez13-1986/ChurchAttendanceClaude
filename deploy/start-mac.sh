#!/bin/bash
# Church Attendance - Start (macOS)

PLIST_NAME="com.churchattendance.app"
PLIST="$HOME/Library/LaunchAgents/$PLIST_NAME.plist"

if launchctl list | grep -q "$PLIST_NAME"; then
    echo "Church Attendance is already running."
else
    launchctl bootstrap "gui/$(id -u)" "$PLIST" 2>/dev/null || launchctl load "$PLIST" 2>/dev/null
    echo "Church Attendance started."
fi
