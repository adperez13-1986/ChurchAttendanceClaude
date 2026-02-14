#!/bin/bash
# Church Attendance App - Deployment Script for Mac/Linux
# This script publishes the app as a self-contained Windows executable

echo "========================================"
echo "Church Attendance - Deployment Script"
echo "========================================"
echo ""

# Clean previous builds
echo "[1/3] Cleaning previous builds..."
dotnet clean -c Release
if [ $? -ne 0 ]; then
    echo "ERROR: Clean failed"
    exit 1
fi

# Publish as self-contained single-file exe for Windows x64
echo ""
echo "[2/3] Publishing application..."
echo "This may take a few minutes..."
dotnet publish ChurchAttendance/ChurchAttendance.fsproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish

if [ $? -ne 0 ]; then
    echo "ERROR: Publish failed"
    exit 1
fi

# Copy data folder to publish directory
echo ""
echo "[3/3] Copying data files..."
cp -r ChurchAttendance/data publish/

# Copy Windows setup scripts
echo "Copying setup scripts..."
cp setup-firewall.bat publish/
cp install-startup.bat publish/
cp README-DEPLOYMENT.md publish/

echo ""
echo "========================================"
echo "Deployment Complete!"
echo "========================================"
echo ""
echo "Published files are in: $(pwd)/publish"
echo ""
echo "NEXT STEPS:"
echo "1. Copy the entire 'publish' folder to a USB drive"
echo "2. On the church Windows PC, copy it to C:\\ChurchAttendance\\"
echo "3. Run 'setup-firewall.bat' (as Administrator) on the church PC"
echo "4. Run 'install-startup.bat' on the church PC to auto-start on boot"
echo "5. Double-click ChurchAttendance.exe to start the app"
echo ""
echo "The app will be accessible at:"
echo "  - On the church PC: http://localhost:5050"
echo "  - On phones/tablets: http://[PC-IP-ADDRESS]:5050"
echo ""
echo "See README-DEPLOYMENT.md for full instructions"
echo ""
