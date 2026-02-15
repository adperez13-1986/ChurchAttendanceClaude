## 1. Backend — Atomic Persistence

- [x] 1.1 Add `toggleAttendanceMember` function to `Database.fs` that takes `date`, `serviceType`, `memberId`, and `checked` — wraps the full read-modify-write in a single `lock lockObj` block
- [x] 1.2 Fix `saveAttendanceRecord` in `Database.fs` to also wrap its full read-modify-write cycle in a single `lock lockObj` block

## 2. Backend — Toggle Endpoint

- [x] 2.1 Add `toggleAttendance` handler in `Handlers.fs` — parses `date`, `serviceType`, `memberId`, `checked` from form data, calls `Database.toggleAttendanceMember`, returns HTML status snippet with updated count
- [x] 2.2 Register `POST /attendance/toggle` route in `Program.fs`

## 3. Frontend — Single-Member Toggle

- [x] 3.1 Update checkbox markup in `Templates.fs` to include a `data-member-id` attribute on each member checkbox so JS can identify which member was toggled
- [x] 3.2 Rewrite `autoSaveAttendance` in `app.js` to send `POST /attendance/toggle` with only the changed member's ID and checked state (plus date and serviceType from the form's hidden fields)
