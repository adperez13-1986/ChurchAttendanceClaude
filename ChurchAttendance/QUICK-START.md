# Church Attendance App - Quick Start

## How to Access

### On the Church Computer (localhost):
- HTTP: http://localhost:5050
- HTTPS: https://localhost:5051 (secure)

### From Other Devices on Church Network:
- HTTP: http://192.168.178.102:5050
- HTTPS: https://192.168.178.102:5051 (secure)

## First Time Using HTTPS?

You'll see a security warning - this is normal for self-signed certificates.

**Click "Advanced" then "Proceed" or "Accept Risk"**

You only need to do this once per device.

## Running the App

```bash
cd /Users/adrianperez/repos/ChurchAttendanceClaude/ChurchAttendance
dotnet run
```

Or to explicitly use HTTPS profile:
```bash
dotnet run --launch-profile https
```

## Why Use HTTPS?

- Encrypts data between browser and server
- Protects member information on the network
- Recommended for any sensitive church data

## Need Help?

See `HTTPS-SETUP.md` for detailed instructions and troubleshooting.
