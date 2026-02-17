## 1. Authentication Middleware Setup

- [x] 1.1 Add cookie authentication services and middleware to `Program.fs` — configure `AddAuthentication().AddCookie()` with 7-day expiration, login path `/login`, and logout path `/logout`
- [x] 1.2 Add `UseAuthentication()` and `UseAuthorization()` to the middleware pipeline, after `UseStaticFiles()` but before route mapping
- [x] 1.3 Add an authorization policy that requires authenticated users, applied to all routes except `/login`

## 2. Password Configuration

- [x] 2.1 Read `APP_PASSWORD` env var at startup with `"changeme"` fallback; log a warning to console if the default is in use

## 3. Login Page and Handlers

- [x] 3.1 Add `loginPage` template to `Templates.fs` — standalone HTML page (no nav layout) with Pico CSS, centered password form, and optional error message
- [x] 3.2 Add `GET /login` handler in `Handlers.fs` that returns the login page template
- [x] 3.3 Add `POST /login` handler that validates the submitted password against the configured value, sets the auth cookie on success (using `HttpContext.SignInAsync`), and redirects to `/`. On failure, re-render login page with "Invalid password" error
- [x] 3.4 Register `GET /login` and `POST /login` routes in `Program.fs` (excluded from auth requirement)

## 4. Logout

- [x] 4.1 Add `POST /logout` handler that calls `HttpContext.SignOutAsync` and redirects to `/login`
- [x] 4.2 Register `POST /logout` route in `Program.fs`
- [x] 4.3 Add a "Logout" button to the nav bar in `Templates.fs` `layout` function — a small form that POSTs to `/logout`

## 5. Verification

- [x] 5.1 Manually verify: unauthenticated request to `/` redirects to `/login`
- [x] 5.2 Manually verify: static files (`/css/app.css`) load without auth
- [x] 5.3 Manually verify: correct password logs in and redirects to dashboard
- [x] 5.4 Manually verify: incorrect password shows error message on login page
- [x] 5.5 Manually verify: logout clears session and redirects to `/login`
