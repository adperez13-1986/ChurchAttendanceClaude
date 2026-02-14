## MODIFIED Requirements

### Requirement: Expandable search
The sticky top bar SHALL contain a search icon button. When tapped, the search icon SHALL reveal a text input that replaces the bar content. Typing in the input SHALL filter member rows across all sections, showing only rows whose name contains the search text (case-insensitive). Sections with matching members SHALL be auto-expanded. Sections with no matches SHALL be hidden. Clearing the input or tapping a close button SHALL restore the normal top bar, show all sections, and restore sections to their collapsed state.

#### Scenario: Open search and filter
- **WHEN** the user taps the search icon and types "ali"
- **THEN** only members whose name contains "ali" (case-insensitive) are visible
- **AND** sections with matching members are auto-expanded
- **AND** sections with no visible members are hidden

#### Scenario: Close search
- **WHEN** the user taps the close button on the search input
- **THEN** the search input is hidden, the normal top bar content is restored, all sections are visible, and sections are restored to collapsed state
