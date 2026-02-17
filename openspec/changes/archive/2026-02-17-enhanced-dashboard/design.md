## Context

The dashboard at `GET /` currently queries total members, active members, and today's attendance count, then renders three stat cards. It provides no historical context, no trends, and no quick navigation. The `Database.getAttendance()` function already returns all attendance records, so recent activity data is available without new queries.

## Goals / Non-Goals

**Goals:**
- Make the dashboard informative at a glance — show what happened recently
- Provide quick actions to reduce clicks for the most common tasks
- Add a fourth stat card for last service attendance
- Style stat cards with colored accent borders to visually differentiate them

**Non-Goals:**
- Charts or graphs (too complex for now, could be a future change)
- Dark mode (separate change)
- Changing the navigation bar or layout structure

## Decisions

### 1. Compute recent activity from existing data
**Choice:** Call `Database.getAttendance()` in the dashboard handler and sort/take the last 5 records.
**Rationale:** The attendance list is small (one record per service per day). Sorting in memory is fine. No need for a new database query or index. Alternative (dedicated "recent" query) is over-engineering for JSON flat files.

### 2. Stat card accent colors via CSS left-border
**Choice:** Add a colored left border to each stat card using CSS classes (e.g., `.stat-card-blue`, `.stat-card-green`, `.stat-card-amber`, `.stat-card-purple`).
**Rationale:** Left-border accent is a common dashboard pattern — visually distinct without being overwhelming. Works with Pico CSS `<article>` elements. Alternative (background tint) was considered but can clash with Pico's theme colors.

### 3. Quick actions as prominent buttons
**Choice:** Two buttons below the stat cards — "Take Today's Attendance" and "View Reports" — linking to `/attendance` and `/reports`.
**Rationale:** These are the two most common actions from the dashboard. Simple `<a>` styled as buttons. No new endpoints needed.

### 4. Last service stat shows most recent service, not today
**Choice:** The fourth stat card shows the headcount from the most recent attendance record (regardless of whether it's today).
**Rationale:** On most days there is no service, so "today's attendance" is often 0. Showing the last recorded service (e.g., "28 — Last Sunday") is more useful. The card includes the date/service label for context.

## Risks / Trade-offs

- **[Performance with large attendance history]** → `getAttendance()` loads all records. With years of data this could be hundreds of records. Sorting and taking 5 is still fast. Mitigation: no action needed now; if it becomes a problem, add a `getRecentAttendance(n)` query later.
- **[Empty state]** → If no attendance records exist, "Recent Activity" and "Last Service" card will be empty. Mitigation: show a friendly empty state message.
