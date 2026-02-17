## Why

The app currently has no authentication. Once exposed through Cloudflare, all endpoints (member roster, attendance records, PDF reports) are accessible to anyone on the internet. A shared password gate keeps unauthorized users out while staying simple enough for church staff who share a single device.

## What Changes

- Add a `/login` page with a single password field
- Add cookie-based session middleware that redirects unauthenticated requests to `/login`
- Password is read from an environment variable (`APP_PASSWORD`) with a fallback default for local dev
- Static files (CSS, JS) are served without auth so the login page renders styled
- Session cookie lasts 7 days so users don't re-enter the password frequently
- Add a logout mechanism to clear the session

## Capabilities

### New Capabilities
- `shared-password-auth`: Cookie-based authentication using a single shared password, with login/logout pages and middleware that gates all app routes

### Modified Capabilities

_(none — no existing spec-level requirements change)_

## Impact

- **Code**: New middleware in `Program.fs`, new login/logout handlers in `Handlers.fs`, new login page template in `Templates.fs`
- **Dependencies**: ASP.NET built-in `Microsoft.AspNetCore.Authentication.Cookies` (already included in the framework, no new NuGet package)
- **Config**: New `APP_PASSWORD` environment variable
- **Data**: No data model changes
