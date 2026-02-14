# Church Attendance App - Deployment Guide

## Prerequisites
- A Windows PC at the church (Windows 10 or 11)
- Network/WiFi that the church PC and phones/tablets share

## Deployment Steps

### On Your Development Machine (Mac)

1. **Build the Windows executable:**
   ```bash
   ./deploy.bat
   ```

   This creates a `publish/` folder with everything needed.

2. **Transfer to Church PC:**
   - Copy the entire `publish/` folder to a USB drive
   - Or use cloud storage, email, etc.

### On the Church Windows PC

3. **Copy Files:**
   - Copy the `publish` folder to the church PC (e.g., `C:\ChurchAttendance\`)
   - You now have: `C:\ChurchAttendance\ChurchAttendance.exe`

4. **Configure Firewall (for phone/tablet access):**
   - Right-click `setup-firewall.bat` → **Run as administrator**
   - This allows port 5050 through Windows Firewall

5. **Install Auto-Startup (optional but recommended):**
   - Double-click `install-startup.bat`
   - The app will now start automatically when Windows boots

6. **Start the App:**
   - Double-click `ChurchAttendance.exe`
   - A console window will appear (leave it open)
   - The app is now running!

## Accessing the App

### From the Church PC:
- Open any browser (Chrome, Edge, Firefox)
- Go to: `http://localhost:5050`

### From Phones/Tablets on Church WiFi:
1. Find the church PC's IP address:
   - On the PC, press `Win+R`, type `cmd`, press Enter
   - Type: `ipconfig`
   - Look for "IPv4 Address" (e.g., `192.168.1.100`)

2. On phone/tablet browser:
   - Go to: `http://192.168.1.100:5050` (use the actual IP)

## Updating the App

1. Run `deploy.bat` again on your Mac
2. Copy the new `publish` folder to the church PC
3. Replace the old folder
4. Restart the app

## Troubleshooting

### Can't access from phones/tablets?
- Make sure phone and PC are on the same WiFi network
- Run `setup-firewall.bat` as administrator
- Check Windows Firewall settings
- Try temporarily disabling antivirus

### App won't start?
- Make sure you copied the entire `publish` folder
- Check that `data/` folder exists with `members.json` and `attendance.json`

### Console window is annoying?
- Minimize it to the taskbar
- Or create a VBS wrapper to hide it (ask if needed)

## Data Location

All data is stored in JSON files in the `data/` folder:
- `data/members.json` - All member records
- `data/attendance.json` - All attendance records
- `data/settings.json` - SMTP email settings

**IMPORTANT:** Back up the `data/` folder regularly!

## Support

For issues or questions, contact the developer.
