## Context

The app is a single-tenant F# ASP.NET Minimal API serving one church. Data is stored as JSON flat files in `~/.church-attendance/data/`. Auth is a single shared password from `APP_PASSWORD` env var. The app runs as a single process on a church desktop (no Docker).

We need to serve two churches (JIL Vienna, JIL Donaustadt) from one process, with separate data and passwords, routed by subdomain.

## Goals / Non-Goals

**Goals:**
- Serve multiple churches from a single process via subdomain routing
- Isolate data completely between tenants (separate directories)
- Per-tenant passwords
- Zero-friction local development (default tenant fallback)
- Landing page at bare domain

**Non-Goals:**
- User accounts or roles (still shared-password per church)
- Database migration (staying with JSON flat files)
- Self-service tenant provisioning (admin edits `tenants.json` manually)
- Cross-tenant analytics or reporting

## Decisions

### 1. Tenant config in JSON file, not env vars

Store tenant configuration in `~/.church-attendance/tenants.json`:

```json
{
  "default": "vienna",
  "tenants": {
    "vienna": { "name": "JIL Vienna", "password": "secretVienna" },
    "donaustadt": { "name": "JIL Donaustadt", "password": "secretDonau" }
  }
}
```

**Why over env vars**: Easier to add tenants without restarting. Keeps config together in one readable file. Env vars get unwieldy with multiple tenants.

**Why over a database**: No new dependency. JSON file matches the existing flat-file pattern. Good enough for <20 tenants.

### 2. Subdomain extraction from Host header

Tenant resolution logic:
1. Parse `Host` header → extract first subdomain label
2. Look up subdomain in `tenants.json`
3. If found → use that tenant
4. If not found (e.g., `localhost`, bare domain) → use `default` tenant from config
5. Store tenant slug in `HttpContext.Items["Tenant"]`

**Why subdomain over path prefix**: Cleaner URLs (`vienna.jilaustria.org/members` vs `jilaustria.org/vienna/members`). No route changes needed. Natural mapping to Cloudflare DNS.

### 3. Per-tenant data directories

```
~/.church-attendance/
├── tenants.json
├── vienna/
│   ├── members.json
│   └── attendance.json
└── donaustadt/
    ├── members.json
    └── attendance.json
```

**Why not `{tenant}/data/`**: The extra `data/` nesting adds nothing. Keep it flat.

### 4. Tenant parameter threading via helper function

Add a `getTenant` helper that reads from `HttpContext.Items`. Each handler calls `getTenant ctx` and passes the slug to Database functions. Database functions accept `tenant: string` as first parameter.

**Why not middleware that sets a scoped service**: Overkill for this app. A simple `Items` dictionary entry is sufficient and doesn't require DI changes.

### 5. Landing page at bare domain

When the `Host` header has no recognized subdomain AND the path is `/login` or `/` (before auth redirect), show a simple page with links to each church's subdomain. This only triggers when the request doesn't resolve to a known tenant — i.e., when accessing the bare domain `jilaustria.org`.

Actually, simpler: the landing page is just another route. When tenant resolution results in "no tenant" (bare domain, no default), serve the landing page. But since we have a default tenant for localhost dev, we need to distinguish:
- `localhost` → default tenant (for dev)
- `jilaustria.org` (bare domain, no subdomain) → landing page

Resolution: Check if Host matches any configured `domain` pattern. The tenants.json can include a `domain` field:

```json
{
  "default": "vienna",
  "domain": "jilaustria.org",
  "tenants": { ... }
}
```

Logic:
- Host = `vienna.jilaustria.org` → tenant `vienna`
- Host = `jilaustria.org` (matches `domain` exactly) → landing page
- Host = anything else (localhost, IP, unknown) → default tenant

### 6. Config loaded once at startup

Read `tenants.json` at app startup. If file doesn't exist, create a default one with a single tenant matching current behavior (slug: "default", password from `APP_PASSWORD` or "changeme"). This preserves backward compatibility — existing deployments without `tenants.json` keep working.

## Risks / Trade-offs

- **[Data migration]** → Existing data in `~/.church-attendance/data/` must be manually moved to `~/.church-attendance/{tenant}/`. Mitigation: print migration instructions at startup if old path exists.
- **[Config file security]** → Passwords in plain text JSON file. Acceptable for now (same security level as env vars on disk). Future: could hash passwords.
- **[No hot reload]** → Adding a tenant requires app restart to pick up config changes. Acceptable for <20 tenants managed by one admin.
- **[Lock contention]** → Single global lock object shared across tenants. Acceptable for low-concurrency church app. Files are in different directories so no actual data contention.
