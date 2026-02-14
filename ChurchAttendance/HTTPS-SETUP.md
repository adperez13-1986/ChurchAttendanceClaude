# HTTPS Setup Guide

This Church Attendance app now supports HTTPS for secure connections on your local network.

## Quick Start

The app runs on both HTTP and HTTPS:

- **HTTP**: http://localhost:5050 or http://192.168.178.102:5050 (backwards compatible)
- **HTTPS**: https://localhost:5051 or https://192.168.178.102:5051 (secure)

## First-Time Browser Setup

When you first access the HTTPS URL, your browser will show a security warning. This is **normal and expected** for self-signed certificates.

### How to Accept the Certificate

#### Chrome/Edge:
1. You'll see "Your connection is not private"
2. Click "Advanced"
3. Click "Proceed to localhost (unsafe)" or "Proceed to 192.168.178.102 (unsafe)"
4. This is a one-time step - the browser will remember your choice

#### Firefox:
1. You'll see "Warning: Potential Security Risk Ahead"
2. Click "Advanced"
3. Click "Accept the Risk and Continue"
4. This is a one-time step

#### Safari:
1. You'll see "This Connection Is Not Private"
2. Click "Show Details"
3. Click "visit this website"
4. Enter your Mac password if prompted
5. This is a one-time step

## Why the Security Warning?

The certificate is "self-signed" - meaning we created it ourselves rather than purchasing one from a trusted authority. This is:

- **Perfect for local/LAN use** (like in a church)
- **Not suitable for public internet** (would need a real certificate)
- **Completely safe** when you know it's your own server

The warning is the browser's way of saying "I don't recognize who signed this certificate" - which is expected since we signed it ourselves.

## Technical Details

### Certificate Information
- **Valid for**: 365 days (1 year)
- **Valid domains**: localhost, *.local
- **Valid IPs**: 127.0.0.1, 192.168.178.102
- **Location**: `/certs/certificate.pfx`

### Ports
- **HTTP Port**: 5050 (kept for backwards compatibility)
- **HTTPS Port**: 5051 (new secure endpoint)

Both ports are available simultaneously, so existing bookmarks and links will continue to work.

## Regenerating the Certificate

If the certificate expires or you need to change the IP address:

1. Navigate to the project directory
2. Run these commands:

```bash
# Create config file with your LAN IP
cat > certs/openssl.cnf << 'EOF'
[req]
default_bits = 2048
prompt = no
default_md = sha256
distinguished_name = dn
x509_extensions = v3_req

[dn]
C = US
ST = Local
L = Local
O = Church Attendance
CN = localhost

[v3_req]
subjectAltName = @alt_names

[alt_names]
DNS.1 = localhost
DNS.2 = *.local
IP.1 = 127.0.0.1
IP.2 = YOUR_LAN_IP_HERE
EOF

# Generate new certificate
openssl req -x509 -newkey rsa:2048 -keyout certs/key.pem -out certs/cert.pem -days 365 -nodes -config certs/openssl.cnf

# Convert to PFX format
openssl pkcs12 -export -out certs/certificate.pfx -inkey certs/key.pem -in certs/cert.pem -passout pass:
```

3. Rebuild and restart the app

## Troubleshooting

### "Certificate not found" error
Make sure the `certs` folder exists in the same directory as the executable. The build process should copy it automatically.

### Certificate still shows as invalid
Clear your browser cache and cookies for the site, then try again.

### Want to use a different port?
Edit `Program.fs` and change the port numbers in the `ConfigureKestrel` section.

## For Production Use

For a public-facing website, you would need:
- A real domain name
- A certificate from a trusted authority (Let's Encrypt is free)
- Proper firewall and security configuration

This setup is intentionally simple for **local church network use only**.
