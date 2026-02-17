## ADDED Requirements

### Requirement: Bottom tab bar on mobile screens
On screens ≤768px, the app SHALL display a fixed bottom tab bar with 4 tabs: Dashboard, Members, Attendance, and Reports. Each tab SHALL show a Unicode icon and a short label.

#### Scenario: Bottom tab bar is visible on mobile
- **WHEN** a user views any authenticated page on a screen ≤768px wide
- **THEN** a fixed bottom tab bar with 4 tabs is visible at the bottom of the screen

#### Scenario: Bottom tab bar is hidden on desktop
- **WHEN** a user views any page on a screen wider than 768px
- **THEN** the bottom tab bar is not visible

### Requirement: Active tab is highlighted
The currently active tab in the bottom tab bar SHALL be visually highlighted using the brand accent color.

#### Scenario: Active tab indicator
- **WHEN** a user is on the Attendance page on mobile
- **THEN** the Attendance tab in the bottom bar is highlighted with the accent color and the other tabs are not

### Requirement: Slim top header on mobile
On screens ≤768px, the top navigation SHALL show only the app title and the theme toggle button. The page links and logout button SHALL be hidden from the top nav.

#### Scenario: Top nav is simplified on mobile
- **WHEN** a user views any authenticated page on a screen ≤768px wide
- **THEN** the top nav shows only "Church Attendance" title and the theme toggle

#### Scenario: Desktop nav is unchanged
- **WHEN** a user views any page on a screen wider than 768px
- **THEN** the full horizontal top nav with all links, theme toggle, and logout is displayed

### Requirement: Logout is accessible on mobile
On mobile screens, the logout action SHALL be available via a link or button at the bottom of the page content area, not in the tab bar.

#### Scenario: Logout on mobile
- **WHEN** a user scrolls to the bottom of any page on mobile
- **THEN** a logout button is visible below the main content

### Requirement: Content is not obscured by bottom tab bar
On mobile screens, page content SHALL have sufficient bottom padding so that no content is hidden behind the fixed bottom tab bar.

#### Scenario: Content fully visible above tab bar
- **WHEN** a user scrolls to the bottom of any page on mobile
- **THEN** all content is visible above the bottom tab bar

### Requirement: Attendance bottom bar sits above tab bar
On mobile screens, the attendance page's sticky bottom bar (View Summary / Share PDF) SHALL be positioned above the bottom tab bar, not overlapping it.

#### Scenario: Attendance actions visible above tab bar
- **WHEN** a user views the attendance checklist on mobile
- **THEN** the View Summary and Share PDF buttons are visible above the bottom tab bar
