#!/bin/bash
# ============================================================
# Church Attendance - macOS Setup Script
# Run this on the target Mac from the publish-mac folder.
# It installs the app, generates HTTPS certs, and creates
# a Launch Agent so the app runs automatically on login.
# ============================================================

set -e

INSTALL_DIR="$HOME/ChurchAttendance"
CERT_DIR="$INSTALL_DIR/certs"
DATA_DIR="$HOME/.church-attendance/data"
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PLIST_NAME="com.churchattendance.app"
PLIST_PATH="$HOME/Library/LaunchAgents/$PLIST_NAME.plist"

echo ""
echo "=== Church Attendance - macOS Setup ==="
echo ""
echo "Install directory : $INSTALL_DIR"
echo "Data directory    : $DATA_DIR"
echo ""

# 1. Copy app files (exclude setup scripts from install dir)
echo "[1/5] Copying app files to $INSTALL_DIR..."
mkdir -p "$INSTALL_DIR"
for f in "$SCRIPT_DIR/"*; do
    case "$(basename "$f")" in
        setup-mac.sh|start-mac.sh|stop-mac.sh) continue ;;
        *) cp -R "$f" "$INSTALL_DIR/" ;;
    esac
done
chmod +x "$INSTALL_DIR/ChurchAttendance"
echo "       Done."

# 2. Create data directory and seed data
echo "[2/5] Setting up data directory..."
mkdir -p "$DATA_DIR"
if [ -d "$SCRIPT_DIR/data" ] && [ ! -f "$DATA_DIR/members.json" ]; then
    cp "$SCRIPT_DIR/data/"*.json "$DATA_DIR/" 2>/dev/null && echo "       Copied seed data." || true
fi
if [ -f "$DATA_DIR/members.json" ]; then
    echo "       Data already present."
fi
echo "       Done."

# 3. Generate self-signed HTTPS certificate
echo "[3/5] Generating HTTPS certificate..."
mkdir -p "$CERT_DIR"

HOSTNAME=$(hostname -s)
openssl req -x509 -newkey rsa:2048 -nodes \
    -keyout "$CERT_DIR/key.pem" \
    -out "$CERT_DIR/cert.pem" \
    -days 3650 \
    -subj "/CN=Church Attendance" \
    -addext "subjectAltName=DNS:localhost,DNS:$HOSTNAME,DNS:$HOSTNAME.local,IP:127.0.0.1" \
    2>/dev/null

# Convert to PFX (what Kestrel expects)
openssl pkcs12 -export -out "$CERT_DIR/certificate.pfx" \
    -inkey "$CERT_DIR/key.pem" \
    -in "$CERT_DIR/cert.pem" \
    -passout pass: \
    2>/dev/null

echo "       Certificate created for: localhost, $HOSTNAME, $HOSTNAME.local"
echo "       Done."

# 4. Trust the certificate (must use the PEM cert, not the PFX bundle)
echo "[4/5] Trusting certificate (may ask for your password)..."
sudo security add-trusted-cert -d -r trustRoot \
    -k /Library/Keychains/System.keychain \
    "$CERT_DIR/cert.pem" 2>/dev/null \
    && echo "       Certificate trusted." \
    || echo "       WARNING: Could not trust certificate. Browsers may show a warning."

# Clean up PEM files (no longer needed — PFX is used by Kestrel, cert was trusted above)
rm -f "$CERT_DIR/key.pem" "$CERT_DIR/cert.pem"

# 5. Create Launch Agent for auto-start on login
echo "[5/5] Setting up auto-start on login..."
mkdir -p "$HOME/Library/LaunchAgents"

cat > "$PLIST_PATH" << PLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>Label</key>
    <string>$PLIST_NAME</string>
    <key>ProgramArguments</key>
    <array>
        <string>$INSTALL_DIR/ChurchAttendance</string>
    </array>
    <key>WorkingDirectory</key>
    <string>$INSTALL_DIR</string>
    <key>RunAtLoad</key>
    <true/>
    <key>KeepAlive</key>
    <true/>
    <key>StandardOutPath</key>
    <string>$INSTALL_DIR/logs/stdout.log</string>
    <key>StandardErrorPath</key>
    <string>$INSTALL_DIR/logs/stderr.log</string>
</dict>
</plist>
PLIST

mkdir -p "$INSTALL_DIR/logs"
echo "       Launch Agent created: $PLIST_PATH"
echo "       Done."

echo ""
echo "=== Setup Complete ==="
echo ""
echo "The app is installed at $INSTALL_DIR"
echo "It will start automatically when you log in."
echo ""
echo "Access the app at:"
echo "  http://localhost:5050       (HTTP)"
echo "  https://localhost:5051      (HTTPS)"
LOCAL_IP=$(ipconfig getifaddr en0 2>/dev/null || echo "[your-ip]")
echo "  http://$LOCAL_IP:5050  (from other devices)"
echo ""

read -p "Start the app now? (y/n) " -n 1 -r
echo ""
if [[ $REPLY =~ ^[Yy]$ ]]; then
    launchctl bootstrap "gui/$(id -u)" "$PLIST_PATH" 2>/dev/null || launchctl load "$PLIST_PATH" 2>/dev/null
    echo "App started. Opening browser..."
    sleep 2
    open "http://localhost:5050"
fi

echo ""
echo "Done!"
