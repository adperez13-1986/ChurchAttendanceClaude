## 1. Remove Email Code

- [x] 1.1 Delete `EmailService.fs` and remove its `<Compile>` entry from `.fsproj`
- [x] 1.2 Remove MailKit `<PackageReference>` from `.fsproj`
- [x] 1.3 Remove `SmtpSettings` type and `defaultSmtpSettings` from `Domain.fs`
- [x] 1.4 Remove `settingsFile`, `getSmtpSettings`, and `saveSmtpSettings` from `Database.fs`

## 2. Remove Handlers and Routes

- [x] 2.1 Remove `emailReport`, `saveSmtpSettings`, and `settingsPage` handlers from `Handlers.fs`
- [x] 2.2 Remove routes: `GET /settings`, `POST /settings/smtp`, `POST /reports/email` from `Program.fs`

## 3. Remove UI

- [x] 3.1 Remove Settings nav link from layout template in `Templates.fs`
- [x] 3.2 Remove settings page template from `Templates.fs`
- [x] 3.3 Remove "Email Report" button from reports page template in `Templates.fs`

## 4. Verify

- [x] 4.1 Build succeeds with no errors or warnings about removed code
