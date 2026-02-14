## Context

The attendance page has a date picker and a hidden `serviceType` input. JavaScript (`updateServiceType`) reads the date, calls `getDay()`, and sets the hidden input to `PrayerMeeting` (Friday) or `SundayService` (all other days). HTMX then includes this in its GET request to `/attendance/list`.

The bug: HTMX's `load` trigger fires before `DOMContentLoaded` completes, so the hidden input is still empty. The server's `parseServiceType` defaults empty strings to `SundayService`.

## Goals / Non-Goals

**Goals:**
- Fix service type detection so it's always correct on page load
- Simplify by removing unnecessary client-side logic

**Non-Goals:**
- Allowing manual service type override (not currently supported, not adding it)

## Decisions

### 1. Move inference to server side
**Decision**: The server will infer service type from the date's day of week when `serviceType` is empty/missing.
**Rationale**: Eliminates the race condition entirely. The server already receives the date — it can compute the day of week itself. This also removes ~30 lines of JS event wiring that only existed to keep the hidden input in sync.

### 2. Remove the hidden input and JS function
**Decision**: Remove the `serviceType` hidden input from the form and the `updateServiceType` function + all its event listeners from app.js.
**Rationale**: With server-side inference, the client no longer needs to send service type. The `serviceType` parameter becomes optional in the query string — if absent or empty, the server computes it.

### 3. Keep serviceType parameter in POST endpoints
**Decision**: The POST endpoints (`saveAttendance`, `autoSaveAttendance`, `exportAttendancePdf`) still receive `serviceType` from the checklist form's hidden input (rendered by the server in `attendanceChecklist`). No change needed there — the server already sets the correct value when rendering the checklist.
**Rationale**: The checklist template already has `<input type="hidden" name="serviceType" value="{serviceType}">` which the server populates correctly. Only the initial GET to `/attendance/list` has the race condition.

## Risks / Trade-offs

- [Server must agree on day-of-week mapping] → Use same logic: Friday (DayOfWeek.Friday) = PrayerMeeting, all others = SundayService
