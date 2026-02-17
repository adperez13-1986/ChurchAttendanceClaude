## Context

The church attendance app is an F# ASP.NET Minimal API app using HTMX + Pico CSS. It currently has zero authentication — all routes are publicly accessible. The app will be exposed to the internet via Cloudflare Tunnel from the church desktop. A simple shared password gate is needed to prevent unauthorized access while keeping the UX frictionless for church staff.

## Goals / Non-Goals

**Goals:**
- Gate all app routes behind a single shared password
- Persist the session via a cookie so users don't re-enter the password frequently
- Keep the login page visually consistent (styled with Pico CSS)
- Read the password from an environment variable for easy configuration

**Non-Goals:**
- Individual user accounts or roles
- Password hashing (single shared password, not stored in a database)
- CSRF protection (separate concern, not part of this change)
- Rate limiting on login attempts (Cloudflare WAF handles this)

## Decisions

### 1. Use ASP.NET Cookie Authentication middleware
**Choice:** `Microsoft.AspNetCore.Authentication.Cookies` (built into the framework)
**Rationale:** Already available in the ASP.NET runtime — no new NuGet packages. Provides signed cookies (tamper-proof), configurable expiration, and automatic redirect to login. The alternative (rolling our own cookie + middleware) would duplicate what the framework already does well.

### 2. Password source: environment variable with fallback
**Choice:** Read from `APP_PASSWORD` env var; fall back to `"changeme"` for local dev.
**Rationale:** Keeps the password out of source code. On the church desktop, set the env var once in the system environment. The fallback makes local development frictionless — no env var needed to run the app. The alternative (JSON config file in `data/`) was considered but env vars are the standard approach and avoid accidentally committing secrets.

### 3. Cookie expiration: 7 days, persistent
**Choice:** `ExpireTimeSpan = 7 days`, `IsPersistent = true`
**Rationale:** Church staff open the app on the same browser weekly. A 7-day cookie means they log in once and it lasts through the next Sunday service. Shorter durations (e.g., session-only) would require re-login every time the browser closes, which is annoying on a shared desktop.

### 4. Static files bypass auth
**Choice:** Place `UseStaticFiles()` before `UseAuthentication()` in the middleware pipeline.
**Rationale:** Static files (CSS, JS) contain no sensitive data. Allowing them through without auth means the login page renders with full Pico CSS styling. The alternative (auth on everything) would show an unstyled login form.

### 5. Login page: standalone template (no nav layout)
**Choice:** The login page uses a minimal HTML template without the app's nav bar.
**Rationale:** The nav bar links to protected routes that would just redirect back to login. Showing nav on the login page is confusing. A clean, centered password form is the right UX.

### 6. Logout: POST /logout clears cookie, redirects to /login
**Choice:** Add a "Logout" link in the nav bar that POSTs to `/logout`.
**Rationale:** POST prevents accidental logout via link prefetching. The nav bar is the natural place for logout since it's visible on every page. After logout, redirect to `/login`.

## Risks / Trade-offs

- **[Single shared password]** → Anyone with the password has full access. Acceptable for a small church team. If the password is compromised, change `APP_PASSWORD` and restart the app — all sessions are invalidated because ASP.NET cookie auth uses a data protection key that can be rotated.
- **[Fallback password in code]** → The `"changeme"` default is visible in source. This is intentional for dev ergonomics but the app should log a warning on startup if the default is in use.
- **[No brute-force protection in app]** → Cloudflare WAF rate limiting is the mitigation. The app itself does not throttle login attempts.
