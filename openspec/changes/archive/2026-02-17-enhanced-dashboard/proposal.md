## Why

The dashboard currently shows three static numbers (total members, active members, today's attendance) and nothing else. It doesn't help church admins understand trends or take action. Enhancing it with recent activity, quick actions, and richer stats makes the app feel complete and gives the landing page real utility.

## What Changes

- Add a fourth stat card: last service attendance count
- Add a "Recent Activity" section showing the last 5 attendance records with date, service type, and headcount
- Add "Quick Actions" buttons linking directly to today's attendance and reports
- Improve stat card styling with colored accent borders to visually differentiate them
- Update the dashboard handler to query recent attendance data

## Capabilities

### New Capabilities
- `enhanced-dashboard`: Richer dashboard with recent attendance activity, quick action links, and improved stat card styling

### Modified Capabilities

_(none — no existing spec-level requirements change)_

## Impact

- **Code**: `Handlers.fs` (dashboard handler needs to query recent attendance), `Templates.fs` (new dashboard template sections), `app.css` (stat card accent styling)
- **Dependencies**: None — uses existing `Database` module queries
- **Data**: No data model changes — reads existing attendance records
