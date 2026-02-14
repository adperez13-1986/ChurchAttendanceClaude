## Context

Data files (`members.json`, `attendance.json`, `settings.json`) are currently stored at `AppContext.BaseDirectory/data/`, which resolves to the build output directory. The `.fsproj` copies seed data with `PreserveNewest`, causing runtime data to be silently overwritten when source timestamps are newer.

## Goals / Non-Goals

**Goals:**
- Data survives `dotnet clean`, `dotnet build`, and `dotnet run` cycles
- Single, predictable data location regardless of how the app is launched

**Non-Goals:**
- Database migration (staying with JSON flat files)
- Multi-user or cloud deployment concerns
- Backward-compatible data migration tooling (manual one-time copy is acceptable)

## Decisions

### Decision 1: Store data in user home directory

Use `~/.church-attendance/data/` as the data directory.

**Alternatives considered:**
- **Environment variable (e.g., `CHURCH_DATA_DIR`)**: More flexible, but adds configuration burden for a single-user app. Can be added later if needed.
- **Project root directory**: Would work for development but is fragile for deployment and ties data to the source tree.
- **`Environment.SpecialFolder.ApplicationData`**: Cross-platform correct (`~/Library/Application Support/` on macOS, `%APPDATA%` on Windows), but the path is less discoverable. For a dev-stage app, a visible dotfile directory is simpler.

**Rationale:** `~/.church-attendance/` is visible, predictable, and completely decoupled from the build output. Easy to find, back up, or delete.

### Decision 2: Remove seed data from source tree

Delete `data/members.json` and `data/attendance.json` from the project directory and remove the `<Content Update="data\**" ...>` line from `.fsproj`. The app already handles missing files gracefully (returns empty lists via `readFile` default values).

### Decision 3: Keep certs in build output

The `certs/` copy rule stays unchanged — certificates are deployment artifacts, not runtime data.

## Risks / Trade-offs

- **Existing data in build output won't auto-migrate** → Document the one-time manual copy in a console message on first run if old data exists but new location is empty.
- **Path hardcoded to `~/.church-attendance/`** → Acceptable for now; can add env var override later if needed.
