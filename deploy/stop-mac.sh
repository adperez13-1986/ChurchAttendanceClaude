#!/bin/bash
# Church Attendance - Stop (macOS)

PLIST_NAME="com.churchattendance.app"
PLIST="$HOME/Library/LaunchAgents/$PLIST_NAME.plist"

if launchctl list | grep -q "$PLIST_NAME"; then
    launchctl bootout "gui/$(id -u)/$PLIST_NAME" 2>/dev/null || launchctl unload "$PLIST" 2>/dev/null
    echo "Church Attendance stopped."
else
    echo "Church Attendance is not running."
fi
