## 1. Bottom Tab Bar HTML

- [x] 1.1 Add a bottom tab bar `<nav>` element to the `layout` function in `Templates.fs` — 5 tabs (Dashboard, Members, Attendance, Reports, More) each with a Unicode icon and label, with the active tab marked via a CSS class
- [x] 1.2 Add a "More" popup menu triggered by the 5th tab — contains Dark/Light Mode toggle and Logout (uses `<div>`/`<a>` elements to avoid Pico CSS button interference)

## 2. CSS Responsive Styles

- [x] 2.1 Add bottom tab bar CSS — fixed position at bottom, flex layout, tab styling with icon + label stacked, active tab accent color
- [x] 2.2 Add `@media (max-width: 768px)` rules: hide top nav links (`.nav-link`) and nav actions (`.nav-action`), show bottom tab bar, add body padding-bottom for tab bar height
- [x] 2.3 On mobile, show only app title in the top nav — theme toggle and logout are in the More menu
- [x] 2.4 On mobile, offset the attendance page's `.attendance-bottom-bar` above the tab bar height
- [x] 2.5 Add More menu CSS — fixed position popup above tab bar, styled menu items with icons and labels

## 3. Desktop Nav

- [x] 3.1 Desktop theme toggle uses inline SVG icons (moon/sun) via JS `innerHTML` — defined as `let` bindings in Templates.fs
- [x] 3.2 Desktop logout shows text "Logout" button (no icon needed)
- [x] 3.3 SVG icon definitions (`svgMoon`, `svgSun`, `svgLogout`) added to Templates.fs for desktop use

## 4. Additional UI Improvements

- [x] 4.1 Members page: replaced grid layout with `.page-header` flex row (heading left, Add Member button right)
- [x] 4.2 Members page: removed "Search by Name" label, kept search input with placeholder
- [x] 4.3 Members page: added `margin-bottom: 1.5rem` to `.page-header` for spacing
- [x] 4.4 Members page: changed Edit/Deactivate text buttons to icon buttons (✎/✕) wrapped in `.action-btns` flex row
- [x] 4.5 Members tab icon changed from ♟ (pawn) to 👥 (people)

## 5. Verification

- [x] 5.1 Build and verify the app compiles
- [x] 5.2 Mobile: bottom tab bar visible with 5 tabs, active tab highlighted
- [x] 5.3 Mobile: top nav shows only "Church Attendance" title
- [x] 5.4 Mobile: More menu opens/closes on tap with Dark Mode toggle and Logout
- [ ] 5.5 Manually verify: on desktop, layout is unchanged (top nav with all links + SVG theme toggle + Logout text, no bottom tab bar)
- [ ] 5.6 Manually verify: attendance bottom bar (View Summary / Share PDF) sits above the tab bar on mobile
