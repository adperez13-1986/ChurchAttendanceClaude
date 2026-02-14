### Requirement: Persistent data directory

The system SHALL store all data files (members, attendance, settings) in `~/.church-attendance/data/`, independent of the application's build output directory.

#### Scenario: Data survives rebuild

- **WHEN** the application is stopped, rebuilt with `dotnet build`, and restarted
- **THEN** all previously saved members, attendance records, and settings MUST still be present

#### Scenario: Data directory created on first run

- **WHEN** the application starts and `~/.church-attendance/data/` does not exist
- **THEN** the system SHALL create the directory automatically
- **AND** the application SHALL start with empty data (no seed data)

### Requirement: Migration hint for existing data

The system SHALL detect when data exists in the old location (`AppContext.BaseDirectory/data/`) but the new location (`~/.church-attendance/data/`) is empty, and log a message to the console telling the user how to migrate.

#### Scenario: Old data exists but new location is empty

- **WHEN** the application starts
- **AND** `AppContext.BaseDirectory/data/members.json` exists with content
- **AND** `~/.church-attendance/data/members.json` does not exist
- **THEN** the system SHALL print a console message with the old and new paths and instructions to copy the files
