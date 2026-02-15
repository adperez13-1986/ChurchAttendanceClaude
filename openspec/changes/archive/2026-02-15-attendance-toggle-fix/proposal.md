## Why

When multiple people take attendance simultaneously (e.g., on separate phones), checkbox ticks are lost. The current auto-save sends the **entire list** of checked members from each browser. If two people tick different members at the same time, the second save overwrites the first — losing data. This is a critical bug in a multi-user environment.

## What Changes

- Replace the "send full form" auto-save with a **single-member toggle** endpoint that adds or removes one member at a time
- Add an atomic read-modify-write lock in `Database.fs` so concurrent toggle requests don't race each other
- Update the frontend `autoSaveAttendance` to send only the changed checkbox (memberId + checked/unchecked) instead of the entire form
- Add a debounce to the frontend toggle to reduce redundant requests from rapid ticking

## Capabilities

### New Capabilities
- `attendance-toggle`: Individual member toggle endpoint and atomic persistence for concurrent-safe attendance marking

### Modified Capabilities
- `compact-attendance-checklist`: Auto-save behaviour changes from sending full form data to sending a single member toggle

## Impact

- **Database.fs**: New `toggleAttendanceMember` function with atomic locking; existing `saveAttendanceRecord` remains for any non-toggle use
- **Handlers.fs**: New `POST /attendance/toggle` route handler
- **Program.fs**: New route registration
- **app.js**: `autoSaveAttendance` rewritten to send single toggle; debounce added
- **Templates.fs**: Checkbox markup may need `data-member-id` attributes for clean JS targeting
- No new dependencies
