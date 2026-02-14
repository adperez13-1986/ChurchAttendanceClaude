## Context

The app currently has email functionality (SMTP-based report sending via MailKit) and a Settings page solely for SMTP configuration. The Share PDF feature using the Web Share API already covers the same use case more flexibly — users can share to any destination from their phone's native share sheet.

## Goals / Non-Goals

**Goals:**
- Remove all email-related code, types, handlers, routes, and UI
- Remove the Settings page entirely (no remaining settings to configure)
- Remove the MailKit dependency to reduce binary size
- Clean removal with no orphaned references

**Non-Goals:**
- Changing the PDF generation or Share PDF functionality
- Adding any replacement for email sending
- Modifying the reports page layout beyond removing the email button

## Decisions

**Pure deletion approach** — No abstractions, no feature flags, no deprecation period. The email feature has no external consumers and no data worth preserving (`settings.json` only stores SMTP credentials). Simply delete all email-related code.

**Remove entire Settings page** — Rather than leaving an empty settings page, remove it completely since SMTP config is its only content. If settings are needed in the future, they can be re-added.

**F# compile order matters** — `EmailService.fs` must be removed from the `<Compile>` list in `.fsproj`. The remaining file order stays the same.

## Risks / Trade-offs

- **No email fallback** → Users must use Share PDF or Download PDF. This is acceptable since Share PDF already supports sending via email apps.
- **settings.json orphaned on existing installs** → Harmless; the file simply won't be read. No migration needed.
