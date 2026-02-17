## Why

On mobile screens, the horizontal top nav bar overflows — "Reports" gets truncated, and the theme toggle and logout button are pushed off-screen. The app is primarily used on phones at the church door for attendance check-in, so mobile navigation must be first-class.

## What Changes

- On mobile (≤768px), hide the top nav links and show a fixed bottom tab bar with 4 tabs: Dashboard, Members, Attendance, Reports
- Each tab uses a simple Unicode icon + short label
- The top bar on mobile becomes a slim header: just the app title + theme toggle
- Logout moves into the page content (bottom of the main area) on mobile, keeping the tab bar clean
- Desktop nav remains unchanged — this is mobile-only via CSS media query
- Active tab is highlighted with the brand accent color

## Capabilities

### New Capabilities
- `mobile-bottom-nav`: Fixed bottom tab bar navigation for mobile screens with icon + label tabs, slim top header, and responsive show/hide via CSS media query

### Modified Capabilities

_(none — desktop behavior unchanged)_

## Impact

- **Code**: `Templates.fs` (add bottom nav HTML + slim mobile header), `app.css` (bottom tab bar styles + media query to toggle top/bottom nav), minor tweak to layout for logout placement
- **Dependencies**: None — uses Unicode characters for icons (no icon library)
- **Data**: No changes
