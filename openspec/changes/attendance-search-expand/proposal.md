## Why

The Members page auto-expands collapsed age group sections when a search matches names inside them, and restores all sections when the search is cleared. The Attendance checklist page only hides/shows sections but never expands collapsed ones, making it impossible to see search results without manually opening each section.

## What Changes

- Update `filterAttendanceRows()` in `app.js` to auto-expand sections with search hits and restore collapsed state when search is cleared, matching the behavior of `filterMemberRows()`

## Capabilities

### New Capabilities

_None_

### Modified Capabilities

- `compact-attendance-checklist`: Search filter should auto-expand sections with matching names and restore sections when search is cleared

## Impact

- **File**: `wwwroot/js/app.js` — `filterAttendanceRows()` function only
