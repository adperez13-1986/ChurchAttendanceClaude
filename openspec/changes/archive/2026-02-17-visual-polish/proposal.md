## Why

The app uses Pico CSS defaults with minimal custom styling. It works but feels like a developer tool. Small CSS tweaks — a brand accent color, better nav styling, softer card shadows, and a dark mode toggle — would make it feel polished and professional for church staff without changing any functionality or adding dependencies.

## What Changes

- Introduce a brand accent color (teal) applied to nav highlight, buttons, and links
- Replace the nav underline active indicator with a colored bottom border
- Add subtle box-shadows and increased border-radius to article/card elements
- Add a dark mode toggle in the nav bar using Pico's built-in `data-theme="dark"` support
- Persist the theme choice in localStorage so it survives page refreshes
- Improve the login page styling to match the new visual language

## Capabilities

### New Capabilities
- `visual-polish`: Brand accent color, refined nav styling, card shadows, and dark mode toggle with localStorage persistence

### Modified Capabilities

_(none — purely visual, no behavior changes)_

## Impact

- **Code**: `app.css` (bulk of changes), `Templates.fs` (dark mode toggle in nav + login page), `app.js` (theme toggle logic + localStorage)
- **Dependencies**: None — uses Pico CSS built-in dark mode
- **Data**: No changes
