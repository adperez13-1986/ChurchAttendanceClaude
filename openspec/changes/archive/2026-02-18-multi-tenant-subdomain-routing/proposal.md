## Why

The app currently serves a single church. We need to support two churches (JIL Vienna and JIL Donaustadt) from a single process running on a church desktop — no Docker, no multiple processes. Subdomain-based routing lets each church have its own data and password while keeping deployment simple.

## What Changes

- Tenant configuration via `~/.church-attendance/tenants.json` mapping slugs to church names and passwords
- Data directories scoped per tenant: `~/.church-attendance/{tenant}/members.json` and `attendance.json`
- Tenant extracted from `Host` header subdomain (e.g., `vienna.jilaustria.org` → `vienna`)
- Default tenant fallback when no subdomain detected (for localhost development)
- Per-tenant passwords replacing the single `APP_PASSWORD` env var
- Landing page at bare domain (`jilaustria.org`) with links to each church's subdomain
- Church name displayed in the app header per tenant

## Capabilities

### New Capabilities
- `multi-tenancy`: Subdomain-based tenant routing, tenant config file, default tenant fallback, and landing page at bare domain

### Modified Capabilities
- `data-storage`: Data directory changes from `~/.church-attendance/data/` to `~/.church-attendance/{tenant}/` — **BREAKING** for existing data (manual migration needed)
- `shared-password-auth`: Password source changes from single `APP_PASSWORD` env var to per-tenant passwords in `tenants.json`

## Impact

- **Database.fs**: All public functions gain a `tenant` parameter; data directory computed per-tenant
- **Handlers.fs**: Every handler extracts tenant from request and passes to Database
- **Program.fs**: Tenant config loading, middleware for tenant extraction, landing page route
- **Templates.fs**: Church name in page header/title
- **Existing data**: Users must move `~/.church-attendance/data/*` → `~/.church-attendance/{tenant}/` and create `tenants.json`
