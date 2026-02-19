## MODIFIED Requirements

### Requirement: Persistent data directory
The system SHALL store all data files (members, attendance) in `~/.church-attendance/{tenant}/`, where `{tenant}` is the resolved tenant slug. Each tenant's data SHALL be completely isolated in its own directory.

#### Scenario: Data survives rebuild
- **WHEN** the application is stopped, rebuilt with `dotnet build`, and restarted
- **THEN** all previously saved members, attendance records, and settings MUST still be present for each tenant

#### Scenario: Data directory created on first run
- **WHEN** the application starts and `~/.church-attendance/{tenant}/` does not exist for a configured tenant
- **THEN** the system SHALL create the directory automatically
- **AND** the tenant SHALL start with empty data (no seed data)

#### Scenario: Tenant data isolation
- **WHEN** data is written for tenant `vienna`
- **THEN** the files SHALL be stored in `~/.church-attendance/vienna/`
- **AND** tenant `donaustadt` data in `~/.church-attendance/donaustadt/` SHALL NOT be affected

### Requirement: Migration hint for existing data
The system SHALL detect when data exists in the old location (`~/.church-attendance/data/`) and the new tenant-scoped locations are empty, and log a message to the console telling the user how to migrate.

#### Scenario: Old data exists but tenant locations are empty
- **WHEN** the application starts
- **AND** `~/.church-attendance/data/members.json` exists with content
- **AND** no tenant directories contain a `members.json` file
- **THEN** the system SHALL print a console message explaining the new tenant-scoped directory structure and instructions to move files to `~/.church-attendance/{tenant}/`
