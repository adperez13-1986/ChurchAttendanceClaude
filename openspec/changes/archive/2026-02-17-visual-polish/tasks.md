## 1. Brand Accent Color

- [x] 1.1 Add `:root` / `[data-theme="light"]` and `[data-theme="dark"]` CSS custom property overrides for `--pico-primary` and related properties using teal (#0d9488)
- [x] 1.2 Update nav active indicator from `text-decoration: underline` to `border-bottom: 2px solid var(--pico-primary)` with padding

## 2. Card Styling

- [x] 2.1 Add `box-shadow` and `border-radius: 8px` to article elements in CSS

## 3. Dark Mode Adaptive Colors

- [x] 3.1 Replace hardcoded hex colors in `app.css` with Pico CSS variables — status messages, attendance top/bottom bars, age group headers, attendance row hover
- [x] 3.2 Update any inline styles in `Templates.fs` that use hardcoded colors to use CSS classes or variables (login page error message, etc.)

## 4. Dark Mode Toggle

- [x] 4.1 Add a theme toggle button to the nav bar in `Templates.fs` `layout` function (between Reports link and Logout button)
- [x] 4.2 Add theme toggle to the login page template as well (top-right corner)
- [x] 4.3 Add theme toggle JavaScript in `app.js` — toggle `data-theme` attribute, update toggle icon, save to `localStorage`
- [x] 4.4 Add inline script in `Templates.fs` layout `<head>` to apply stored theme from `localStorage` before first paint (prevents flash of wrong theme)

## 5. Verification

- [x] 5.1 Build and verify the app compiles
- [x] 5.2 Manually verify: teal accent color on buttons, links, and active nav
- [x] 5.3 Manually verify: dark mode toggle works and persists after refresh
- [x] 5.4 Manually verify: all custom elements (status messages, sticky bars, cards) render correctly in dark mode
- [x] 5.5 Manually verify: login page renders correctly in both themes
