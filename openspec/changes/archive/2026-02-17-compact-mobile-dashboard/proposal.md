## Why

On mobile screens, the dashboard's 4 stat cards stack vertically at full width, each with large text and Pico CSS article padding. This pushes the action buttons ("Take Today's Attendance", "View Reports") and recent activity table below the fold, requiring excessive scrolling just to reach the primary actions.

## What Changes

- Replace Pico's `.grid` with a custom `.stat-grid` that renders as a 2x2 grid on mobile
- Shrink stat card styling on mobile: smaller font size, reduced padding, compact labels
- Desktop layout remains unchanged (4 columns)

## Capabilities

### New Capabilities
- `compact-mobile-dashboard`: Mobile-optimized 2x2 stat grid with compact card styling

### Modified Capabilities

## Impact

- `ChurchAttendance/Templates.fs` — Dashboard `homePage` function: change stat card wrapper from `div.grid` to `div.stat-grid`, adjust card markup for compact labels
- `ChurchAttendance/wwwroot/css/app.css` — Add `.stat-grid` styles and mobile overrides for stat cards
