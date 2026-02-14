## 1. Server-Side Inference

- [x] 1.1 Add `inferServiceType` helper in Handlers.fs that returns PrayerMeeting for Friday, SundayService otherwise
- [x] 1.2 Update `attendanceList` handler to use `inferServiceType` when `serviceType` query param is empty

## 2. Remove Client-Side Logic

- [x] 2.1 Remove the hidden `serviceType` input and the `serviceType` query param from the HTMX form in `attendancePage` (Templates.fs)
- [x] 2.2 Remove `updateServiceType` function and all its event listeners from app.js (DOMContentLoaded, htmx:afterSwap, htmx:configRequest, change on #date)
