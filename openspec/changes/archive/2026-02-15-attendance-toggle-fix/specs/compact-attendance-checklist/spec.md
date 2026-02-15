## MODIFIED Requirements

### Requirement: Auto-save behaviour preserved
Toggling any member checkbox SHALL trigger an auto-save request to `POST /attendance/toggle` with the individual member's ID and checked state (`date`, `serviceType`, `memberId`, `checked`). The auto-save status SHALL be displayed in the sticky bottom bar. Each toggle SHALL send only the changed member, not the full form data.

#### Scenario: Auto-save on checkbox toggle
- **WHEN** a member checkbox is toggled
- **THEN** a POST request is sent to `/attendance/toggle` with `memberId` set to the toggled member's ID and `checked` set to the checkbox state
- **AND** the save status is shown in the sticky bottom bar

#### Scenario: Multiple users toggling simultaneously
- **WHEN** two users toggle different member checkboxes at the same time for the same date/service
- **THEN** both toggles are saved independently and neither overwrites the other
