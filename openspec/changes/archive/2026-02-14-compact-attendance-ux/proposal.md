## Why

The attendance checklist page has too much visual chrome before the first checkbox appears (~420px of headers, counters, and filters). Ushers are used to paper sheets — simple name lists with tick boxes, grouped by age group. The current UI requires excessive scrolling, buries the Share PDF action at the bottom, and shows columns (Age Group, Category) that aren't useful during the act of ticking attendance.

## What Changes

- **Replace** the date banner, attendance counter block, and filter inputs with a single compact sticky top bar showing service info and live count
- **Replace** the full table layout (checkbox + Name + Age Group + Category columns) with a simple checkbox + name list
- **Group** members by age group using section headers with per-group counts (e.g., "Adults (8/30)") instead of a dropdown filter
- **Move** the name search behind a search icon in the sticky top bar (expandable on tap, hidden by default)
- **Remove** the age group dropdown filter (replaced by visible sections), first timer badge, and select-all checkbox from the ticking view
- **Move** Share PDF and View Summary buttons to a sticky bottom bar, always visible regardless of scroll position

## Capabilities

### New Capabilities
- `compact-attendance-checklist`: Compact, paper-like attendance ticking experience with sticky top/bottom bars, age-group sections, and hidden search

### Modified Capabilities
_None. The `member-list-filtering` spec covers the Members page, not the attendance page. No existing spec requirements change._

## Impact

- **Templates.fs**: Attendance checklist HTML rendering (the `attendanceChecklist` function) — major rewrite of the markup structure
- **app.css**: New styles for sticky top bar, sticky bottom bar, section headers, compact name rows, expandable search
- **app.js**: Update attendance counter logic, search filtering, auto-save wiring to work with new DOM structure; remove select-all logic from attendance page
- No backend/API changes — the same form fields (`memberIds` checkboxes, `date`, `serviceType` hidden inputs) are submitted
- No data model changes
