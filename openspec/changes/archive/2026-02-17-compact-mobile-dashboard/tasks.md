## 1. Template Changes

- [x] 1.1 In `Templates.fs` `homePage`, replace the stat cards' wrapper `div.grid` with `div.stat-grid` and add a `<small class="stat-label">` below each `.stat` paragraph for the compact mobile label
- [x] 1.2 Ensure each stat card retains its existing `<header>` and accent class for desktop

## 2. CSS

- [x] 2.1 Add `.stat-grid` CSS — desktop: 4-column grid (matching current layout), mobile (≤768px): 2-column grid
- [x] 2.2 Add mobile overrides: hide `.stat-grid article header`, reduce `.stat` font-size to ~1.5rem, reduce article padding
- [x] 2.3 Style `.stat-label` as small centered text below the number (visible on mobile, hidden on desktop)

## 3. Verification

- [x] 3.1 Build and verify the app compiles
- [ ] 3.2 Manually verify: desktop dashboard unchanged (4 stat cards in a row)
- [ ] 3.3 Manually verify: mobile dashboard shows 2x2 stat grid with compact cards
- [ ] 3.4 Manually verify: action buttons visible without scrolling past cards on mobile
