# Accessing Church Attendance via Hostname

## Overview

The Church Attendance application is now configured to work with your Mac's hostname, making it accessible regardless of network changes or IP address assignments. This guide explains how to access the application using the hostname instead of IP addresses.

## Your Mac's Hostname

Your Mac's local hostname is: **Adrians-MacBook-Pro-2**

## How to Access the Application

You can access the Church Attendance application using any of these URLs:

1. **Using hostname (Recommended):**
   ```
   https://Adrians-MacBook-Pro-2.local:5051
   ```

2. **Using localhost (only on this Mac):**
   ```
   https://localhost:5051
   ```

## Why Hostname Access is Better

### Advantages:
- **Network Independent**: Works on any network (home, office, public WiFi) without reconfiguration
- **No IP Changes**: Your IP address can change, but your hostname stays the same
- **Easy to Remember**: Easier to remember than an IP address
- **Automatic Discovery**: Uses mDNS/Bonjour for automatic discovery on the local network
- **Multiple Devices**: Other devices on the same network can access using the same URL

### How It Works:
- The `.local` suffix uses mDNS (Multicast DNS), also known as Bonjour on macOS
- Devices on the same local network automatically resolve `*.local` hostnames
- No DNS server configuration needed
- Works across different network configurations

## Finding Your Hostname

If you ever need to check your Mac's hostname, use these commands:

```bash
# Get the local hostname
scutil --get LocalHostName

# Get the full hostname
hostname
```

## Accessing from Other Devices

Other devices on the same local network (phones, tablets, other computers) can access the application using:

```
https://Adrians-MacBook-Pro-2.local:5051
```

**Note**: You may need to accept the self-signed certificate warning on each device the first time you access the application.

## Certificate Details

The HTTPS certificate includes the following valid names:
- localhost
- Adrians-MacBook-Pro-2
- Adrians-MacBook-Pro-2.local
- *.local (wildcard for any .local domain)

This ensures secure HTTPS connections work properly regardless of how you access the application.

## Troubleshooting

### "Cannot connect" errors:
1. Verify the application is running
2. Check that both devices are on the same network
3. Try using the full hostname with `.local` suffix

### Certificate warnings:
- This is normal for self-signed certificates
- You can safely accept the certificate on trusted devices
- The warning appears once per device/browser

### Hostname not resolving:
- Ensure mDNS/Bonjour is enabled (it's on by default on macOS, iOS, and most modern systems)
- On Windows, you may need to install Bonjour Print Services
- On Linux, ensure Avahi daemon is running

## Technical Details

The certificate is configured in `/Users/adrianperez/repos/ChurchAttendanceClaude/ChurchAttendance/certs/openssl.cnf` and includes:
- RSA 2048-bit encryption
- SHA-256 signature
- 365-day validity
- Subject Alternative Names (SAN) for all supported hostnames
