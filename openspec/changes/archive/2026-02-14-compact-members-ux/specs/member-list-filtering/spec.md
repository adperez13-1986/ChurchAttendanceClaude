## REMOVED Requirements

### Requirement: Members page age group filter
**Reason**: Replaced by visible age group sections. Users can see all groups at a glance and expand the one they need instead of selecting from a dropdown.
**Migration**: Tap the age group section header to expand it.

## MODIFIED Requirements

### Requirement: Members page name search
The members page SHALL display a text search input that filters the members table by name. Filtering SHALL be case-insensitive and match partial names. Filtering SHALL happen on each keystroke without a server request. When filtering, sections with matching members SHALL be shown with matching rows visible, regardless of collapsed state. Sections with no matching members SHALL be hidden.

#### Scenario: Search by partial name
- **WHEN** user types "mar" in the name search input
- **THEN** only members whose name contains "mar" (case-insensitive) are visible, and their parent sections are shown

#### Scenario: Clear search
- **WHEN** user clears the name search input
- **THEN** all sections are shown in their current collapsed/expanded state and all rows are visible within expanded sections

#### Scenario: Search hides empty sections
- **WHEN** user types a name that matches no members in the "Infants" group
- **THEN** the "Infants" section is hidden entirely

### Requirement: Filter reset on table update
When the members table is refreshed via HTMX (after add, edit, or deactivate), the name search input SHALL reset to empty and all rows SHALL be visible.

#### Scenario: Filter reset after adding a member
- **WHEN** user has an active name search and adds a new member
- **THEN** the search input is cleared and all members are visible

## REMOVED Requirements

### Requirement: Combined filtering
**Reason**: The age group dropdown filter has been removed, so combined filtering no longer applies. Name search works across all sections.
**Migration**: Use name search alone. Age groups are visible as sections.
