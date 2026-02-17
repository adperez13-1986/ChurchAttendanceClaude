## 1. Dashboard Handler

- [x] 1.1 Update `Handlers.dashboard` to query recent attendance records — sort by date descending, take the last 5, and compute the last service headcount/label
- [x] 1.2 Pass recent activity data and last service info to the template function

## 2. Dashboard Template

- [x] 2.1 Add a fourth stat card for "Last Service" showing headcount, date, and service type label (with empty state "—" / "No services yet")
- [x] 2.2 Add "Recent Activity" section below the stat cards — a list of up to 5 recent attendance entries showing formatted date, service type, and headcount (with empty state message)
- [x] 2.3 Add "Quick Actions" section with two buttons: "Take Today's Attendance" (links to `/attendance`) and "View Reports" (links to `/reports`)

## 3. Stat Card Styling

- [x] 3.1 Add CSS classes for stat card accent colors — `.stat-blue`, `.stat-green`, `.stat-amber`, `.stat-purple` — each applying a distinct colored left border
- [x] 3.2 Apply the accent classes to the four stat cards in the dashboard template

## 4. Verification

- [x] 4.1 Build and verify the app compiles
- [x] 4.2 Manually verify: dashboard shows all four stat cards with colored accents
- [x] 4.3 Manually verify: recent activity section displays attendance records
- [x] 4.4 Manually verify: quick action buttons link to correct pages
- [x] 4.5 Manually verify: empty state displays correctly when no attendance data exists
