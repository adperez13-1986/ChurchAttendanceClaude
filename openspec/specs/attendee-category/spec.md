### Requirement: Attendee category exists
The system SHALL provide an `Attendee` category for people who attend regularly but are not official members.

#### Scenario: Attendee appears in category list
- **WHEN** the system enumerates all categories
- **THEN** the list SHALL be Member, Attendee, UnderMonitoring, Visitor (in that order)

### Requirement: Attendee is selectable when adding or editing a person
The system SHALL include `Attendee` as an option in the category dropdown on the member form.

#### Scenario: Adding a new person as Attendee
- **WHEN** a user creates a new person and selects "Attendee" as the category
- **THEN** the person SHALL be saved with category `Attendee`

#### Scenario: Editing an existing person to Attendee
- **WHEN** a user edits an existing person and changes their category to "Attendee"
- **THEN** the person's category SHALL be updated to `Attendee`

### Requirement: Attendee displays correctly in all views
The system SHALL display "Attendee" as the label for the Attendee category in the members table, attendance checklist, and PDF reports.

#### Scenario: Members table shows Attendee category
- **WHEN** viewing the members table with a person whose category is Attendee
- **THEN** the category column SHALL display "Attendee"

#### Scenario: Attendance checklist shows Attendee category
- **WHEN** viewing the attendance checklist with a person whose category is Attendee
- **THEN** the category column SHALL display "Attendee"

#### Scenario: PDF report shows Attendee category
- **WHEN** generating a PDF report that includes a person whose category is Attendee
- **THEN** the category column SHALL display "Attendee" and the category summary SHALL include an Attendee count

### Requirement: Attendee serializes and deserializes as a string
The system SHALL serialize the Attendee category as `"Attendee"` in JSON and deserialize `"Attendee"` back to the Attendee category.

#### Scenario: Round-trip through JSON
- **WHEN** a person with category Attendee is saved to a JSON file and loaded back
- **THEN** the category SHALL be `Attendee`
