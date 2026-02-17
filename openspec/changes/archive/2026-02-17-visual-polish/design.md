## Context

The app uses Pico CSS v2 with a `data-theme="light"` attribute on `<html>`. Pico natively supports `data-theme="dark"` — switching the attribute is all that's needed to toggle dark mode. The current `app.css` has ~320 lines of custom styles on top of Pico. The nav uses `text-decoration: underline` for the active link.

## Goals / Non-Goals

**Goals:**
- Cohesive visual identity with a brand accent color
- Dark mode that respects user preference and persists across sessions
- Polished card/article elements with softer shadows
- Better active nav indicator

**Non-Goals:**
- Custom icon set or illustrations
- Responsive layout changes (already works on mobile)
- Changing the Pico CSS version or switching frameworks
- Animating transitions between themes

## Decisions

### 1. Brand color: teal (#0d9488)
**Choice:** Teal as the primary accent — applied via Pico CSS custom properties (`--pico-primary`).
**Rationale:** Warm but professional. Works well in both light and dark modes. Avoids religious connotations of specific colors. Pico allows overriding `--pico-primary` and related properties to theme the entire app.

### 2. Dark mode via Pico data-theme attribute
**Choice:** Toggle `data-theme` between `"light"` and `"dark"` on `<html>`. Store choice in `localStorage`.
**Rationale:** Pico handles all the color inversions. We just flip the attribute. No custom dark-mode CSS needed beyond ensuring our custom styles use Pico CSS variables (not hardcoded hex colors). The toggle is a small button in the nav bar.

### 3. Nav active indicator: colored bottom border
**Choice:** Replace `text-decoration: underline` with a `border-bottom: 2px solid var(--pico-primary)` on the active nav link.
**Rationale:** More modern look, consistent with the accent color. The underline style looks like a hyperlink rather than a navigation indicator.

### 4. Card styling: subtle shadow + border-radius
**Choice:** Add `box-shadow: 0 1px 3px rgba(0,0,0,0.08)` and `border-radius: 8px` to `article` elements.
**Rationale:** Gives cards subtle depth without being heavy. Pico's default articles are flat with a border — a slight shadow makes them feel more tactile.

### 5. Update hardcoded colors to use CSS variables
**Choice:** Replace hardcoded hex colors in `app.css` (e.g., `#f8f9fa`, `#155724`, `#d4edda`) with Pico CSS variables where possible, so they adapt in dark mode.
**Rationale:** Hardcoded light-mode colors will look wrong in dark mode. Using `var(--pico-background-color)`, `var(--pico-muted-color)`, etc. ensures consistency.

## Risks / Trade-offs

- **[Hardcoded colors in Templates.fs]** → Some inline styles use hex values (e.g., the login page error message color). These need to be updated to use CSS classes or variables. Mitigation: audit and replace during implementation.
- **[Pico variable overrides]** → Overriding `--pico-primary` affects all Pico components (buttons, links, form focus rings). This is intentional but means we commit to teal everywhere. Mitigation: acceptable — consistent branding is the goal.
