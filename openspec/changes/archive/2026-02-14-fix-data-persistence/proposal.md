## Why

Application data (members, attendance records, settings) is stored under `AppContext.BaseDirectory/data/`, which resolves to the build output directory (`bin/Debug/net10.0/`). The project's `.fsproj` copies seed data files with `CopyToOutputDirectory="PreserveNewest"`, which overwrites runtime data whenever the source file has a newer timestamp — causing silent data loss on rebuild.

## What Changes

- Move data storage to a stable, user-scoped directory outside the build output (e.g., `~/.church-attendance/data/`)
- Remove seed data files from the project's `data/` directory
- Remove the `CopyToOutputDirectory` rule for data files from `.fsproj`

## Capabilities

### New Capabilities

- `data-storage`: Data files are stored in a persistent, build-independent location

### Modified Capabilities

_(none — existing specs cover email/reporting behavior, not storage location)_

## Impact

- `ChurchAttendance/Database.fs`: Change `dataDir` path resolution
- `ChurchAttendance/ChurchAttendance.fsproj`: Remove data content copy rule
- `ChurchAttendance/data/`: Remove seed data files from source tree
