# report-emailing Specification

## Purpose
TBD - created by archiving change standardize-null-checks. Update Purpose after archive.
## Requirements
### Requirement: SMTP host validation

The system SHALL use `String.IsNullOrWhiteSpace` to validate the SMTP host before attempting to send a report. A whitespace-only host MUST be treated as unconfigured.

#### Scenario: SMTP host is whitespace-only

- **WHEN** the SMTP host setting contains only whitespace
- **THEN** the system SHALL treat it as unconfigured
- **AND** display an error prompting the user to configure SMTP settings

