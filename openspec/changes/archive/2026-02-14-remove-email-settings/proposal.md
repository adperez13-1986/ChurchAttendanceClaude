## Why

The Download PDF and Share PDF features fully cover the report distribution use case. The email functionality adds complexity (SMTP configuration, MailKit dependency, settings page) without providing value beyond what Share PDF already offers — which lets users send to any destination (WhatsApp, Messenger, email) from the phone's native share sheet.

## What Changes

- Remove the entire email/SMTP functionality (EmailService, email handlers, email UI)
- Remove the Settings page (it exists solely for SMTP configuration)
- Remove the MailKit NuGet dependency
- Remove the `settings.json` data file and related database functions
- Remove Settings navigation link from the app layout

## Capabilities

### New Capabilities

_None — this is a removal._

### Modified Capabilities

- `email-sending`: **REMOVED** — entire capability deleted
- `report-emailing`: **REMOVED** — entire capability deleted

## Impact

- **Files deleted**: `EmailService.fs`
- **Types removed**: `SmtpSettings`, `defaultSmtpSettings` from Domain.fs
- **Functions removed**: `getSmtpSettings`, `saveSmtpSettings` from Database.fs; `emailReport`, `saveSmtpSettings`, `settingsPage` from Handlers.fs; settings template from Templates.fs
- **Routes removed**: `GET /settings`, `POST /settings/smtp`, `POST /reports/email`
- **UI removed**: Settings nav link, "Email Report" button on reports page
- **Dependencies removed**: MailKit package from `.fsproj`
- **Data**: `settings.json` file no longer created or read
