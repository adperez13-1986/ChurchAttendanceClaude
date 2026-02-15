## ADDED Requirements

### Requirement: Toggle endpoint
The system SHALL expose a `POST /attendance/toggle` endpoint that accepts `date`, `serviceType`, `memberId`, and `checked` (boolean) as form fields. The endpoint SHALL add or remove a single member from the attendance record for the given date and service type.

#### Scenario: Add a member via toggle
- **WHEN** a POST is made to `/attendance/toggle` with `date=2026-02-15`, `serviceType=SundayService`, `memberId=<guid>`, `checked=true`
- **AND** the attendance record for that date/service exists with members [A, B]
- **THEN** the record SHALL be updated to [A, B, <guid>]
- **AND** the response SHALL return an HTML snippet with the updated count

#### Scenario: Remove a member via toggle
- **WHEN** a POST is made to `/attendance/toggle` with `date=2026-02-15`, `serviceType=SundayService`, `memberId=<guid>`, `checked=false`
- **AND** the attendance record for that date/service exists with members [A, B, <guid>]
- **THEN** the record SHALL be updated to [A, B]
- **AND** the response SHALL return an HTML snippet with the updated count

#### Scenario: Toggle on nonexistent record
- **WHEN** a POST is made to `/attendance/toggle` with `checked=true`
- **AND** no attendance record exists for that date/service
- **THEN** the system SHALL create a new attendance record with `MemberIds = [<guid>]`

#### Scenario: Add an already-present member
- **WHEN** a POST is made to `/attendance/toggle` with `checked=true`
- **AND** the member is already in the attendance record's MemberIds
- **THEN** the record SHALL remain unchanged (no duplicate added)

### Requirement: Atomic read-modify-write
The `toggleAttendanceMember` function SHALL lock the entire read-modify-write cycle with a single lock acquisition. The lock SHALL NOT be released between reading the attendance file, modifying the member list, and writing the file back.

#### Scenario: Concurrent toggles for different members
- **WHEN** two toggle requests arrive simultaneously for the same date/service but different members (Alice and Bob)
- **THEN** both members SHALL be present in the final record — neither toggle SHALL overwrite the other

#### Scenario: Concurrent toggles for the same member
- **WHEN** two toggle requests arrive simultaneously for the same member with `checked=true`
- **THEN** the member SHALL appear exactly once in the final record

### Requirement: Atomic locking for saveAttendanceRecord
The existing `saveAttendanceRecord` function SHALL also be updated to lock the entire read-modify-write cycle atomically, preventing races on any code path that still uses it.

#### Scenario: Full-save does not race with toggle
- **WHEN** a full save via `saveAttendanceRecord` and a toggle via `toggleAttendanceMember` arrive simultaneously
- **THEN** both operations SHALL be serialized by the lock and no data SHALL be lost
