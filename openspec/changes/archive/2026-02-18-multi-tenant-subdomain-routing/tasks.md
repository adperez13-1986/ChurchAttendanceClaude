## 1. Tenant Configuration

- [x] 1.1 Add `TenantConfig` and `TenantsConfig` types to Domain.fs (slug, name, password, default, domain)
- [x] 1.2 Add tenant config loading to Database.fs — read `~/.church-attendance/tenants.json` at startup, create default if missing (using `APP_PASSWORD` env var or "changeme")
- [x] 1.3 Expose loaded config via a module-level value (e.g., `Database.tenantConfig`)

## 2. Per-Tenant Data Directories

- [x] 2.1 Change `Database.dataDir` from fixed path to a function `dataDirFor (tenant: string)` returning `~/.church-attendance/{tenant}/`
- [x] 2.2 Add `tenant: string` parameter to all Database public functions (getMembers, saveMembers, getMember, addMember, updateMember, deactivateMember, getAttendance, saveAttendance, getAttendanceForDate, saveAttendanceRecord, toggleAttendanceMember, getFirstTimerIds)
- [x] 2.3 Update `ensureDataDir` to create directories for all configured tenants
- [x] 2.4 Update migration hint to detect old `~/.church-attendance/data/` and print instructions for new tenant-scoped layout

## 3. Tenant Routing

- [x] 3.1 Add tenant resolution logic in Program.fs — extract subdomain from `Host` header, look up in config, store in `HttpContext.Items["Tenant"]`
- [x] 3.2 Add `getTenant` helper in Handlers.fs to read tenant from `HttpContext.Items["Tenant"]`
- [x] 3.3 Detect bare domain (Host matches `domain` config exactly) and serve landing page instead

## 4. Update Handlers

- [x] 4.1 Update all handlers in Handlers.fs to call `getTenant ctx` and pass tenant to Database functions
- [x] 4.2 Update `loginPost` to validate password against the tenant's configured password instead of global `appPassword`
- [x] 4.3 Pass church name (from tenant config) to Templates for display

## 5. Update Templates

- [x] 5.1 Update `Templates.layout` to accept and display church name in the header/title
- [x] 5.2 Thread church name parameter through all page templates that use layout
- [x] 5.3 Create landing page template listing all tenants with subdomain links

## 6. Verification

- [x] 6.1 Test locally with default tenant (localhost:5050 should work as before)
- [x] 6.2 Verify data isolation — different tenants read/write separate directories
- [x] 6.3 Verify per-tenant passwords work correctly
- [x] 6.4 Verify backward compatibility — app works without tenants.json (auto-creates default)
