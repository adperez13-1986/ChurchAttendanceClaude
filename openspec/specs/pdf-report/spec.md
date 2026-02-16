### Requirement: PDF attendees grouped by age group
The PDF attendance report SHALL group attendees by age group, rendering each group under a section header. Sections SHALL appear in the order: Men, Women, YAN, CYN, Children, Infants. Members within each section SHALL be sorted alphabetically by full name. Empty age groups SHALL be omitted.

#### Scenario: Grouped attendees in PDF
- **WHEN** a PDF report is generated for a service with attendees across multiple age groups
- **THEN** attendees are displayed under their respective age group section headers, sorted alphabetically within each section

#### Scenario: Empty age group omitted
- **WHEN** a PDF report is generated and no attendees belong to the "Infants" age group
- **THEN** the "Infants" section SHALL NOT appear in the PDF

### Requirement: Per-group section header with count
Each age group section header in the PDF SHALL display the group name and the number of attendees in that group, formatted as "{group name} ({count})".

#### Scenario: Section header with count
- **WHEN** a PDF report is generated and the "Men" group has 12 attendees
- **THEN** the section header displays "Men (12)" in bold

### Requirement: Per-group numbering
Each age group section SHALL number its members starting at 1. The table columns SHALL be: #, Name, Category.

#### Scenario: Numbering restarts per group
- **WHEN** a PDF report is generated with 5 Men and 3 Women attendees
- **THEN** the Men section numbers 1-5 and the Women section numbers 1-3

### Requirement: First timer highlighting preserved in grouped layout
First timers SHALL continue to be highlighted with a yellow background and asterisk in their name, within their respective age group section.

#### Scenario: First timer in grouped PDF
- **WHEN** a PDF report is generated and a member in the "YAN" group is a first timer
- **THEN** that member's row in the YAN section has a yellow background and their name is suffixed with " *"
