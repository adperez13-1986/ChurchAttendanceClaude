## Context

The current `layout` function in `Templates.fs` renders a single `<nav>` with horizontal links. On desktop this works fine, but on phones the 6+ items overflow. The attendance page is the most-used screen on mobile — someone standing at the church door checking people in. Navigation must be thumb-friendly and always visible.

## Goals / Non-Goals

**Goals:**
- Bottom tab bar on mobile with 4 pages always one tap away
- Slim top header on mobile with just title + theme toggle
- Desktop nav completely unchanged
- Active tab visually indicated

**Non-Goals:**
- Hamburger menu (adds friction — extra tap to see nav)
- Icon library or SVG icons (Unicode characters are sufficient and zero-dependency)
- Changing the page structure or routing
- Animations or transitions on tab switching

## Decisions

### 1. CSS-only responsive toggle
**Choice:** Use `@media (max-width: 768px)` to hide the desktop nav links and show the bottom tab bar. Both HTML elements exist in the DOM; CSS controls visibility.
**Rationale:** No JavaScript needed for show/hide. The 768px breakpoint matches the existing mobile breakpoint in `app.css`. Alternative (JavaScript-driven responsive nav) adds unnecessary complexity.

### 2. Unicode icons for tabs
**Choice:** Use Unicode characters: `⌂` (home/dashboard), `♟` (members/people), `✓` (attendance/check), `📊` (reports/chart).
**Rationale:** Zero dependencies, works everywhere, no icon font to load. The labels below each icon provide clarity. Alternative (icon library like Font Awesome or Lucide) adds a dependency for 4 icons.

### 3. Bottom tab bar is fixed position
**Choice:** `position: fixed; bottom: 0` with a `z-index` above content but below modals.
**Rationale:** Tab bar must always be visible, even when scrolling through long attendance lists. Need `padding-bottom` on `<body>` to prevent content from being hidden behind the tab bar.

### 4. Logout moves to page footer on mobile
**Choice:** On mobile, the logout button is hidden from the top nav and a small "Logout" link appears at the bottom of `<main>` content.
**Rationale:** Logout is rarely used — it doesn't deserve a spot in the 4-tab bar. Keeping it accessible but out of the way is the right trade-off. On desktop, logout stays in the top nav as-is.

### 5. Top header on mobile: title + theme toggle only
**Choice:** On mobile, the `<nav>` shrinks to just the app title and the theme toggle button. The page links are hidden (they're in the bottom tab bar instead).
**Rationale:** Frees up vertical space. The title provides branding, and the theme toggle needs to remain accessible since it's not in the tab bar.

## Risks / Trade-offs

- **[Body padding for fixed bottom bar]** → Content at the bottom of pages could be obscured by the tab bar. Mitigation: add `padding-bottom` to `body` on mobile via the media query.
- **[Attendance bottom bar + tab bar overlap]** → The attendance page has its own sticky bottom bar (View Summary / Share PDF). On mobile, this needs to sit above the tab bar. Mitigation: increase the `bottom` offset of the attendance bottom bar on mobile.
