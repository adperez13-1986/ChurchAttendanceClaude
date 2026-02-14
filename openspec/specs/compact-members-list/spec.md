### Requirement: Age group sections on members page
The members page SHALL group members by age group, rendering each group under a collapsible section. Sections SHALL appear in the order: Men, Women, YAN, CYN, Children, Infants. Members within each section SHALL be sorted with active members first (alphabetically), then inactive members (alphabetically).

#### Scenario: Members grouped by age group
- **WHEN** the members page loads
- **THEN** members are displayed under their respective age group section headers

#### Scenario: Empty age group
- **WHEN** an age group has no members
- **THEN** that age group section SHALL NOT be rendered

### Requirement: Collapsible sections default collapsed
Each age group section SHALL be collapsible. Tapping the section header SHALL toggle between collapsed and expanded states. All sections SHALL default to collapsed when the page loads or after an HTMX table refresh.

#### Scenario: Default collapsed state
- **WHEN** the members page loads
- **THEN** all age group sections are collapsed, showing only headers with counts

#### Scenario: Expand a section
- **WHEN** the user taps a collapsed section header
- **THEN** that section expands to show its member rows

### Requirement: Per-section member count
Each age group section header SHALL display the total number of members in that group, formatted as "{group name} ({count})".

#### Scenario: Section header count
- **WHEN** the members page loads and the "Men" group has 37 members
- **THEN** the section header displays "Men (37)"

### Requirement: Member table columns preserved
Each section SHALL contain a table with columns: Name, Category, Status, Actions. The Age Group column SHALL be omitted since the section header provides that information.

#### Scenario: Table within section
- **WHEN** the user expands a section
- **THEN** the members are displayed in a table with Name, Category, Status, and Actions columns
