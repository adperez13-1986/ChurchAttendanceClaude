## MODIFIED Requirements

### Requirement: Password is configured via tenant config
The system SHALL read the shared password for each tenant from `~/.church-attendance/tenants.json` instead of the `APP_PASSWORD` environment variable. Each tenant has its own password.

#### Scenario: Tenant password is configured
- **WHEN** tenant `vienna` has password `"secretVienna"` in `tenants.json`
- **THEN** the system SHALL accept `"secretVienna"` as the valid password when logging into `vienna.jilaustria.org`

#### Scenario: Different tenants have different passwords
- **WHEN** tenant `vienna` has password `"secretVienna"` and tenant `donaustadt` has password `"secretDonau"`
- **THEN** submitting `"secretVienna"` on `donaustadt.jilaustria.org` SHALL be rejected

#### Scenario: Fallback when no tenants.json exists
- **WHEN** `tenants.json` does not exist (auto-created with defaults)
- **THEN** the system SHALL accept the password from `APP_PASSWORD` env var or `"changeme"` as fallback
