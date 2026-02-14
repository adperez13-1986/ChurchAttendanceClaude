## ADDED Requirements

### Requirement: SMTP credential validation

The system SHALL use `String.IsNullOrWhiteSpace` to validate SMTP username and password before attempting authentication. Whitespace-only credentials MUST be treated as absent.

#### Scenario: SMTP credentials are whitespace-only

- **WHEN** the SMTP username or password contains only whitespace
- **THEN** the system SHALL skip authentication
- **AND** behave the same as if no credentials were provided
