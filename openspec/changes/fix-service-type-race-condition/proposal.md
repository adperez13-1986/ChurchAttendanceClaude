## Why

On the attendance page, the service type (Sunday Service vs Prayer Meeting) is auto-detected from the selected date's day of week. This detection runs in JavaScript (`updateServiceType`), but there is a race condition on page load: HTMX fires the `load` trigger and sends the form before JS has populated the hidden `serviceType` input. The server receives an empty string, defaults to `SundayService`, and renders the wrong service banner. This means Fridays incorrectly show "Sunday Service" on initial load.

## What Changes

- Move service type inference from client-side JS to server-side F#, eliminating the race condition entirely
- The server will determine the service type from the date's day of week when `serviceType` is empty or missing
- Remove the client-side `updateServiceType` JS function and its event wiring (no longer needed)
- Remove the hidden `serviceType` input from the attendance form

## Capabilities

### New Capabilities

### Modified Capabilities

## Impact

- `Handlers.fs`: `attendanceList` and `saveAttendance`/`autoSaveAttendance` handlers will infer service type from date when not provided
- `Templates.fs`: `attendancePage` — remove hidden `serviceType` input
- `app.js`: Remove `updateServiceType` function and all its event listeners
