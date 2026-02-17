## ADDED Requirements

### Requirement: Dashboard displays four stat cards
The dashboard SHALL display four stat cards: Total Members, Active Members, Today's Attendance, and Last Service attendance. Each card SHALL have a distinct colored left-border accent.

#### Scenario: All four stat cards are visible
- **WHEN** an authenticated user visits `/`
- **THEN** the dashboard displays four cards showing total member count, active member count, today's attendance count, and the most recent service attendance count with its date and service type label

#### Scenario: No attendance records exist
- **WHEN** no attendance records exist in the system
- **THEN** the Last Service card displays "—" with the label "No services yet"

### Requirement: Dashboard displays recent attendance activity
The dashboard SHALL display a "Recent Activity" section listing the 5 most recent attendance records, sorted by date descending. Each entry SHALL show the date, service type label, and number of attendees.

#### Scenario: Recent activity with records
- **WHEN** attendance records exist
- **THEN** the dashboard shows up to 5 most recent records, each displaying the formatted date (e.g., "Sun, Feb 16"), the service type (e.g., "Sunday Service"), and the headcount

#### Scenario: No attendance records exist
- **WHEN** no attendance records exist
- **THEN** the Recent Activity section displays a message "No attendance records yet"

### Requirement: Dashboard displays quick action buttons
The dashboard SHALL display two quick action buttons: "Take Today's Attendance" linking to `/attendance` and "View Reports" linking to `/reports`.

#### Scenario: Quick actions are displayed
- **WHEN** an authenticated user visits `/`
- **THEN** the dashboard shows "Take Today's Attendance" and "View Reports" as clickable buttons below the stat cards

### Requirement: Stat cards have colored accent borders
Each stat card SHALL have a visually distinct colored left border to differentiate it from the others.

#### Scenario: Stat cards are visually differentiated
- **WHEN** an authenticated user views the dashboard
- **THEN** each of the four stat cards has a different colored left border (e.g., blue, green, amber, purple)
