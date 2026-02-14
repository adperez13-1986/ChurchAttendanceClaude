# HTTPS Implementation Summary

## Changes Made

### 1. Certificate Generation
Created a self-signed certificate valid for:
- `localhost` (for local access)
- `192.168.178.102` (for LAN access)
- Valid for 365 days

**Files:**
- `/certs/certificate.pfx` - Certificate file used by the app
- `/certs/cert.pem` - Certificate in PEM format (for reference)
- `/certs/key.pem` - Private key (git-ignored for security)
- `/certs/openssl.cnf` - OpenSSL configuration (git-ignored)

### 2. Application Configuration

**Program.fs:**
- Added `System.IO` namespace for path handling
- Added `Microsoft.AspNetCore.Server.Kestrel.Core` for HTTP protocols
- Replaced `UseUrls()` with `ConfigureKestrel()` for dual HTTP/HTTPS support
- HTTP endpoint on port 5050 (backwards compatible)
- HTTPS endpoint on port 5051 (new secure endpoint)
- Certificate loaded from `certs/certificate.pfx` (no password)

### 3. Build Configuration

**ChurchAttendance.fsproj:**
- Added content directive to copy `certs/**` to output directory
- Certificate is automatically deployed with the application

**Properties/launchSettings.json:**
- Kept existing `http` profile for HTTP-only mode
- Added new `https` profile for dual HTTP/HTTPS mode

### 4. Version Control

**.gitignore:**
- Excludes `.pem` files (private keys)
- Excludes `openssl.cnf`
- Includes `.pfx` for convenience (since it's password-less, this is acceptable for LAN-only use)

### 5. Documentation

**HTTPS-SETUP.md:**
- Comprehensive guide for users
- Browser-specific instructions for accepting the certificate
- Certificate regeneration instructions
- Troubleshooting tips

**QUICK-START.md:**
- Simple reference card
- Quick access URLs
- Basic usage instructions

## URLs

### HTTP (Backwards Compatible):
- Local: http://localhost:5050
- LAN: http://192.168.178.102:5050

### HTTPS (New Secure):
- Local: https://localhost:5051
- LAN: https://192.168.178.102:5051

## Running the App

### Default (both HTTP and HTTPS):
```bash
dotnet run
```

### HTTP only:
```bash
dotnet run --launch-profile http
```

### Both HTTP and HTTPS (explicit):
```bash
dotnet run --launch-profile https
```

## Security Considerations

### Appropriate For:
- Local church network use
- Internal applications
- Development/testing
- Small trusted networks

### NOT Appropriate For:
- Public internet deployment
- Production websites
- Untrusted networks
- Applications handling highly sensitive data without additional security layers

### Why This Approach:
1. **Simple**: No certificate authority needed
2. **Free**: No certificate costs
3. **Sufficient**: Provides encryption for LAN traffic
4. **Practical**: One-time browser warning is acceptable for church volunteers

## Testing

The application was successfully built with these changes:
```
dotnet build
```

Output shows:
- No compilation errors
- Certificate successfully copied to output directory
- All dependencies resolved correctly

## Next Steps for Users

1. Run the application: `dotnet run`
2. Access via HTTPS: `https://localhost:5051` or `https://192.168.178.102:5051`
3. Accept the security warning (one-time per browser/device)
4. Bookmark the HTTPS URL for future use

## Certificate Renewal

The certificate is valid for 365 days. To renew:
1. Follow instructions in `HTTPS-SETUP.md` under "Regenerating the Certificate"
2. Rebuild the application
3. Users will need to accept the new certificate (one-time warning again)

## Backward Compatibility

All existing HTTP bookmarks and links will continue to work on port 5050. The HTTPS endpoint is additive and doesn't break existing functionality.
