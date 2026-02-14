#!/bin/bash
# Church Attendance App - Deployment Script for macOS
# Publishes the app as a self-contained macOS binary

echo "========================================"
echo "Church Attendance - Mac Deployment"
echo "========================================"
echo ""

# Detect or override architecture
# Usage: ./deploy-mac.sh [arm64|x64]
if [ "$1" = "x64" ]; then
    RID="osx-x64"
    echo "Target architecture: x64 (Intel) — specified via argument"
elif [ "$1" = "arm64" ]; then
    RID="osx-arm64"
    echo "Target architecture: arm64 (Apple Silicon) — specified via argument"
else
    ARCH=$(uname -m)
    if [ "$ARCH" = "arm64" ]; then
        RID="osx-arm64"
    else
        RID="osx-x64"
    fi
    echo "Target architecture: $ARCH ($RID) — detected from this machine"
    echo "  Tip: pass 'arm64' or 'x64' to target a different Mac"
fi
echo ""

# Clean previous builds
echo "[1/3] Cleaning previous builds..."
dotnet clean -c Release
if [ $? -ne 0 ]; then
    echo "ERROR: Clean failed"
    exit 1
fi

# Publish as self-contained single-file for macOS
echo ""
echo "[2/3] Publishing application for $RID..."
echo "This may take a few minutes..."
dotnet publish ChurchAttendance/ChurchAttendance.fsproj \
    -c Release \
    -r "$RID" \
    --self-contained true \
    -p:PublishSingleFile=true \
    -o publish-mac

if [ $? -ne 0 ]; then
    echo "ERROR: Publish failed"
    exit 1
fi

# Copy setup scripts and data
echo ""
echo "[3/3] Copying setup files..."
cp deploy/setup-mac.sh publish-mac/
cp deploy/start-mac.sh publish-mac/
cp deploy/stop-mac.sh publish-mac/
chmod +x publish-mac/*.sh
chmod +x publish-mac/ChurchAttendance

if [ -d ChurchAttendance/data ]; then
    cp -r ChurchAttendance/data publish-mac/
fi

echo ""
echo "========================================"
echo "Deployment Complete!"
echo "========================================"
echo ""
echo "Published files are in: $(pwd)/publish-mac"
echo ""
echo "NEXT STEPS:"
echo "1. Copy the 'publish-mac' folder to the target Mac"
echo "2. Open Terminal in that folder"
echo "3. Run: ./setup-mac.sh"
echo ""
echo "The app will be accessible at:"
echo "  - On the Mac: http://localhost:5050"
echo "  - On phones/tablets: http://[MAC-IP]:5050"
echo ""
