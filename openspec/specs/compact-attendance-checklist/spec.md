### Requirement: Age group sections
The attendance checklist SHALL group members by age group, rendering each group under a section header. Sections SHALL appear in the order: Men, Women, YAN, CYN, Children, Infants. Members within each section SHALL be sorted alphabetically by full name. Only active members SHALL be included.

#### Scenario: Members grouped by age group
- **WHEN** the attendance checklist loads with members across multiple age groups
- **THEN** members are displayed under their respective age group section headers, sorted alphabetically within each section

#### Scenario: Empty age group
- **WHEN** an age group has no active members
- **THEN** that age group section SHALL NOT be rendered

### Requirement: Per-section attendance count
Each age group section header SHALL display the count of checked members out of total members in that group, formatted as "{group name} ({checked}/{total})". The count SHALL update immediately when a checkbox is toggled.

#### Scenario: Count updates on checkbox toggle
- **WHEN** a member's checkbox is toggled in the "Men" section which has 12 total members and 5 were checked
- **THEN** the section header updates to display "Men (6/12)" if checked, or "Men (4/12)" if unchecked

### Requirement: Compact member rows
Each member SHALL be rendered as a checkbox with the member's full name as a label. The row SHALL NOT display age group, category, or first timer badge. Each row SHALL include a `data-name` attribute with the lowercase full name for search filtering.

#### Scenario: Member row content
- **WHEN** the attendance checklist renders a member named "Alice Smith"
- **THEN** the row displays only a checkbox and the text "Alice Smith", with no additional columns or badges

### Requirement: Sticky top bar
The attendance checklist SHALL display a sticky top bar that remains visible at the top of the scrollable area. The bar SHALL contain: the service type label (abbreviated), the date, the total attendance count, and a search icon button. The bar SHALL use `position: sticky` with `top: 0`.

#### Scenario: Top bar content
- **WHEN** the attendance checklist loads for Sunday Service on 2026-02-14 with 12 members checked
- **THEN** the sticky top bar displays "Sun Service", the date "Feb 14", and "12 present", along with a search icon

#### Scenario: Top bar count updates
- **WHEN** a member checkbox is toggled
- **THEN** the total count in the sticky top bar updates immediately

### Requirement: Expandable search
The sticky top bar SHALL contain a search icon button. When tapped, the search icon SHALL reveal a text input that replaces the bar content. Typing in the input SHALL filter member rows across all sections, showing only rows whose name contains the search text (case-insensitive). Clearing the input or tapping a close button SHALL restore the normal top bar and show all rows.

#### Scenario: Open search and filter
- **WHEN** the user taps the search icon and types "ali"
- **THEN** only members whose name contains "ali" (case-insensitive) are visible, and sections with no visible members are hidden

#### Scenario: Close search
- **WHEN** the user taps the close button on the search input
- **THEN** the search input is hidden, the normal top bar content is restored, and all member rows are visible again

### Requirement: Sticky bottom bar
The attendance checklist SHALL display a sticky bottom bar that remains visible at the bottom of the scrollable area. The bar SHALL contain the "View Summary" submit button, the "Share PDF" button, and the auto-save status indicator. The bar SHALL use `position: sticky` with `bottom: 0`.

#### Scenario: Bottom bar always visible
- **WHEN** the user scrolls through the attendance checklist
- **THEN** the View Summary and Share PDF buttons remain visible at the bottom of the viewport

### Requirement: Auto-save behaviour preserved
Toggling any member checkbox SHALL trigger an auto-save request to `/attendance/auto-save` with the full form data. The auto-save status SHALL be displayed in the sticky bottom bar. This behaviour SHALL be identical to the existing auto-save implementation.

#### Scenario: Auto-save on checkbox toggle
- **WHEN** a member checkbox is toggled
- **THEN** a POST request is sent to `/attendance/auto-save` with the form data, and the save status is shown in the sticky bottom bar

### Requirement: Past-date confirmation preserved
When the selected date is not today, the checklist SHALL first display a confirmation prompt asking the user to confirm recording attendance for a past date. The compact checklist SHALL only be shown after confirmation. This behaviour SHALL be identical to the existing implementation.

#### Scenario: Past date confirmation
- **WHEN** the attendance checklist loads for a past date
- **THEN** a confirmation prompt is displayed, and the checklist is hidden until the user confirms

### Requirement: Collapsible age group sections
Each age group section SHALL be collapsible. Tapping the section header SHALL toggle between collapsed (members hidden) and expanded (members visible) states. All sections SHALL default to collapsed when the checklist loads.

#### Scenario: Default collapsed state
- **WHEN** the attendance checklist loads
- **THEN** all age group sections are collapsed, showing only headers with counts

#### Scenario: Expand a section
- **WHEN** the user taps a collapsed section header
- **THEN** that section expands to show its member rows
