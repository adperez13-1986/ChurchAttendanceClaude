## ADDED Requirements

### Requirement: Members page age group filter
The members page SHALL display a dropdown filter that allows filtering the members table by age group. The dropdown SHALL include an "All" option (default) and one option per age group (Men, Women, YAN, CYN, Children, Infants). Filtering SHALL happen client-side without a server request.

#### Scenario: Filter by specific age group
- **WHEN** user selects "Women" from the age group dropdown
- **THEN** only members with age group "Women" are visible in the table

#### Scenario: Show all members
- **WHEN** user selects "All" from the age group dropdown
- **THEN** all members are visible in the table

### Requirement: Members page name search
The members page SHALL display a text search input that filters the members table by name. Filtering SHALL be case-insensitive and match partial names. Filtering SHALL happen on each keystroke without a server request.

#### Scenario: Search by partial name
- **WHEN** user types "mar" in the name search input
- **THEN** only members whose name contains "mar" (case-insensitive) are visible

#### Scenario: Clear search
- **WHEN** user clears the name search input
- **THEN** all members are visible (subject to age group filter)

### Requirement: Combined filtering
The age group filter and name search SHALL work together. Only members matching both the selected age group AND the search text SHALL be visible.

#### Scenario: Filter by age group and name simultaneously
- **WHEN** user selects "Men" from the age group dropdown AND types "jo" in the name search
- **THEN** only members in the "Men" age group whose name contains "jo" are visible

### Requirement: Filter reset on table update
When the members table is refreshed via HTMX (after add, edit, or deactivate), the filters SHALL reset to their default values (age group "All", empty search) and all rows SHALL be visible.

#### Scenario: Filters reset after adding a member
- **WHEN** user has an active age group filter and adds a new member
- **THEN** the age group dropdown resets to "All", the search input is cleared, and all members are visible
