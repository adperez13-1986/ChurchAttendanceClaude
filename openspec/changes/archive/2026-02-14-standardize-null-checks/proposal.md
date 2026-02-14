## Why

The codebase inconsistently uses `String.IsNullOrEmpty` and `String.IsNullOrWhiteSpace` for validating string values. Whitespace-only strings (e.g. `"   "`) slip through `IsNullOrEmpty` checks, which could lead to invalid SMTP configuration being treated as valid.

## What Changes

- Standardize all string-empty checks to use `String.IsNullOrWhiteSpace`
- Affects 3 call sites across 2 files (`Handlers.fs`, `EmailService.fs`)

## Capabilities

### New Capabilities

_(none)_

### Modified Capabilities

- `email-sending`: SMTP credential and host validation now catches whitespace-only values
- `report-emailing`: SMTP host check before sending now catches whitespace-only values

## Impact

- `ChurchAttendance/Handlers.fs`: 1 call site (line 398)
- `ChurchAttendance/EmailService.fs`: 2 call sites (lines 40-41)
