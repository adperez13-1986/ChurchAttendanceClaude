# Church Attendance Application - Business Rules

## 1. Member Management

### 1.1 Member Data Model
- **BR-1.1.1**: Each member has an `Id` (GUID), `FullName` (string), `AgeGroup`, `Category`, `DateRegistered` (DateTime), `FirstAttendedDate` (optional DateTime), and `IsActive` (boolean).
- **BR-1.1.2**: `Id` is a system-generated GUID assigned at creation time via `Guid.NewGuid()`.
- **BR-1.1.3**: `DateRegistered` is automatically set to `DateTime.Today` at creation time and cannot be modified by the user.
- **BR-1.1.4**: `IsActive` is automatically set to `true` at creation time.
- **BR-1.1.5**: `FirstAttendedDate` is optional. If provided, it is used to identify first timers.

### 1.2 Age Groups
- **BR-1.2.1**: Valid age groups are: `Men`, `Women`, `YAN`, `CYN`, `Children`, `Infants`.
- **BR-1.2.2**: The canonical order is: Men, Women, YAN, CYN, Children, Infants (used for display and reporting).
- **BR-1.2.3**: If no age group is provided during creation, the default is `Men`.
- **BR-1.2.4**: Age group parsing is case-sensitive (e.g., "Men" is valid, "men" is not).

### 1.3 Categories
- **BR-1.3.1**: Valid categories are: `Member`, `Visitor`, `UnderMonitoring`.
- **BR-1.3.2**: Display labels are: "Member", "Visitor", "Under Monitoring" (note: "UnderMonitoring" is the serialized/form value, "Under Monitoring" is the display label).
- **BR-1.3.3**: If no category is provided during creation, the default is `Member`.
- **BR-1.3.4**: Category parsing is case-sensitive.

### 1.4 Creating a Member
- **BR-1.4.1**: Required fields: `FullName` (non-empty), `AgeGroup`, `Category`.
- **BR-1.4.2**: Optional fields: `FirstAttendedDate`.
- **BR-1.4.3**: The system auto-generates `Id`, `DateRegistered` (today), and `IsActive` (true).
- **BR-1.4.4**: If `FullName` is empty/missing from form, it defaults to empty string `""`.
- **BR-1.4.5**: If `AgeGroup` is missing/invalid from form, it defaults to `Men`.
- **BR-1.4.6**: If `Category` is missing/invalid from form, it defaults to `Member`.
- **BR-1.4.7**: If `FirstAttendedDate` is empty or unparseable, it is set to `None`.
- **BR-1.4.8**: New members are appended to the end of the member list.

### 1.5 Editing a Member
- **BR-1.5.1**: Editable fields: `FullName`, `AgeGroup`, `Category`, `FirstAttendedDate`.
- **BR-1.5.2**: Non-editable fields: `Id`, `DateRegistered`, `IsActive` (IsActive is only changed via deactivation).
- **BR-1.5.3**: If a form field is missing during update, the existing value is preserved.
- **BR-1.5.4**: Member lookup is by GUID. If the GUID is invalid or not found, an error message is returned.

### 1.6 Deactivating a Member (Soft Delete)
- **BR-1.6.1**: Deactivation sets `IsActive = false` on the member record. The member is never physically deleted.
- **BR-1.6.2**: Deactivation requires a valid member GUID. If the GUID is invalid, an error message is returned.
- **BR-1.6.3**: If the GUID is valid but no member is found, the operation silently does nothing.
- **BR-1.6.4**: There is no reactivation mechanism in the current system (once deactivated, it requires direct data manipulation or future feature).
- **BR-1.6.5**: Deactivation triggers a confirmation dialog ("Deactivate {name}?") via `hx-confirm`.

### 1.7 Member Display and Sorting
- **BR-1.7.1**: In the members table, members are sorted by: inactive members last, then alphabetically by `FullName`.
- **BR-1.7.2**: Inactive members are displayed with reduced opacity (CSS class `inactive`, `opacity: 0.5`).
- **BR-1.7.3**: Only active members have a "Deactivate" button. Inactive members do not show this button.
- **BR-1.7.4**: All members (active and inactive) have an "Edit" button.

## 2. Attendance Tracking

### 2.1 Attendance Data Model
- **BR-2.1.1**: An attendance record has: `Id` (GUID), `Date` (DateTime), `ServiceType`, and `MemberIds` (list of GUIDs).
- **BR-2.1.2**: The unique key for an attendance record is the combination of `Date` (date portion only) and `ServiceType`.

### 2.2 Service Types
- **BR-2.2.1**: Valid service types are: `SundayService` and `PrayerMeeting`.
- **BR-2.2.2**: Display labels are: "Sunday Service" and "Prayer Meeting".
- **BR-2.2.3**: If an invalid or missing service type is provided, the default is `SundayService`.

### 2.3 Service Type Auto-Inference
- **BR-2.3.1**: The service type is automatically inferred from the selected date's day of week.
- **BR-2.3.2**: If the date falls on a Friday (day of week = 5), service type is set to `PrayerMeeting`.
- **BR-2.3.3**: For all other days (including Sunday = 0), service type is set to `SundayService`.
- **BR-2.3.4**: The service type is recalculated on: page load, date input change, HTMX content swap, and before HTMX request dispatch (`htmx:configRequest`).
- **BR-2.3.5**: The service type is stored in a hidden input field (`#serviceType`) and is not directly editable by the user.

### 2.4 Attendance Date Selection
- **BR-2.4.1**: The date input defaults to today's date (`DateTime.Today`).
- **BR-2.4.2**: The date input has a `max` attribute set to today's date, preventing selection of future dates.
- **BR-2.4.3**: An optional `date` query parameter can override the initial date (used when navigating back from the summary page).

### 2.5 Past Date Confirmation
- **BR-2.5.1**: If the selected date is not today, the attendance checklist is initially hidden.
- **BR-2.5.2**: A confirmation prompt is displayed: "You are about to record attendance for a past date. Continue?"
- **BR-2.5.3**: The user must click "Yes, continue" to reveal the attendance checklist.
- **BR-2.5.4**: The banner displays differently for today (green, class `today`) vs past dates (yellow, class `past-date`, with "(not today)" suffix).

### 2.6 Attendance Checklist
- **BR-2.6.1**: Only active members (`IsActive = true`) are shown in the attendance checklist.
- **BR-2.6.2**: Members in the checklist are sorted alphabetically by `FullName`.
- **BR-2.6.3**: Each member row displays: checkbox, full name, age group label, and category label.
- **BR-2.6.4**: If a previously saved attendance record exists for the selected date and service type, the corresponding member checkboxes are pre-checked.
- **BR-2.6.5**: Each member row has data attributes for filtering: `data-age-group` and `data-name` (lowercase).

### 2.7 First Timer Detection
- **BR-2.7.1**: A member is considered a "first timer" for a given date if their `FirstAttendedDate` matches that date (date portion comparison only).
- **BR-2.7.2**: First timers who are checked (present) in the attendance list are displayed with a `<mark>First Timer</mark>` badge next to their name.
- **BR-2.7.3**: First timers who are not checked do not receive the badge.
- **BR-2.7.4**: First timer status is determined by comparing `member.FirstAttendedDate.Date` with the attendance `date.Date`.

### 2.8 Attendance Filtering
- **BR-2.8.1**: Members can be filtered by name using a search input (`#name-filter`). The search is case-insensitive substring matching.
- **BR-2.8.2**: Members can be filtered by age group using a dropdown (`#age-group-filter`) with options: All, Men, Women, YAN, CYN, Children, Infants.
- **BR-2.8.3**: Both filters work simultaneously -- a row must match both the name filter and the age group filter to be visible.
- **BR-2.8.4**: Filtering is applied client-side by toggling `row.style.display` between `''` and `'none'`.
- **BR-2.8.5**: The name filter triggers on every keystroke (`input` event).
- **BR-2.8.6**: The age group filter triggers on `change` event.

### 2.9 Select All Checkbox
- **BR-2.9.1**: The "Select All" checkbox (`#select-all`) only toggles checkboxes of currently visible rows (rows not hidden by filters).
- **BR-2.9.2**: Hidden rows (filtered out) are not affected by Select All.
- **BR-2.9.3**: Toggling Select All triggers an auto-save and updates the attendance counter.

### 2.10 Attendance Counter
- **BR-2.10.1**: The attendance counter displays the total number of checked members.
- **BR-2.10.2**: When an age group filter is active, the counter shows both visible checked count and total checked count in the format: "{visible} present ({total} total)".
- **BR-2.10.3**: When no filter is active, it shows: "{total} present".
- **BR-2.10.4**: The counter is updated on: checkbox change, select-all toggle, filter change, and after the attendance list HTMX load completes.

### 2.11 Auto-Save
- **BR-2.11.1**: Every checkbox change (individual or select-all) triggers an automatic save via `POST /attendance/auto-save`.
- **BR-2.11.2**: Auto-save uses `fetch()` (not HTMX) with the form data from the attendance form.
- **BR-2.11.3**: While saving, the status area shows "Saving...".
- **BR-2.11.4**: On success, the server responds with a success message showing the count: "Saved ({count} present)".
- **BR-2.11.5**: On failure, the status area shows "Save failed".

### 2.12 Saving Attendance (Upsert Logic)
- **BR-2.12.1**: When saving an attendance record, the system checks for an existing record with the same `Date` (date portion) and `ServiceType`.
- **BR-2.12.2**: If a matching record exists, it is replaced (updated) with the new record.
- **BR-2.12.3**: If no matching record exists, the new record is appended to the list.
- **BR-2.12.4**: The record's `Id` is always a new GUID (even on update, the old Id is replaced).
- **BR-2.12.5**: The `MemberIds` list contains only valid GUIDs parsed from the form; invalid values are silently discarded.

### 2.13 Attendance Summary (View Summary)
- **BR-2.13.1**: Clicking "View Summary" (`POST /attendance`) saves the attendance and displays a summary.
- **BR-2.13.2**: The summary includes: date, service type label, total present count.
- **BR-2.13.3**: The summary includes a breakdown by age group (Men, Women, YAN, CYN, Children, Infants) with counts.
- **BR-2.13.4**: The summary includes a breakdown by category (Member, Visitor, Under Monitoring) with counts.
- **BR-2.13.5**: If any attendees are first timers, a "First Timers" section lists their names.
- **BR-2.13.6**: The summary shows a success message: "Attendance saved successfully!".
- **BR-2.13.7**: A "Back to Attendance" button navigates to `/attendance?date={date}` to preserve the selected date.

## 3. Dashboard

### 3.1 Dashboard Statistics
- **BR-3.1.1**: The dashboard displays three statistics: Total Members, Active Members, and Today's Attendance.
- **BR-3.1.2**: Total Members = count of all members (active and inactive).
- **BR-3.1.3**: Active Members = count of members where `IsActive = true`.
- **BR-3.1.4**: Today's Attendance = count of distinct member IDs across all attendance records for today's date (all service types combined).

## 4. Reporting

### 4.1 Report Date Range
- **BR-4.1.1**: Reports are generated for a date range defined by `startDate` and `endDate`.
- **BR-4.1.2**: The default start date is 7 days ago (`DateTime.Today.AddDays(-7)`).
- **BR-4.1.3**: The default end date is today (`DateTime.Today`).
- **BR-4.1.4**: Both start and end dates are required for PDF generation and email.

### 4.2 PDF Report Generation
- **BR-4.2.1**: The PDF is generated using QuestPDF with the Community license.
- **BR-4.2.2**: The PDF page size is A4 with 1.5 cm margins.
- **BR-4.2.3**: The report header shows "Church Attendance Report" and the date range.
- **BR-4.2.4**: Attendance records are filtered to include only those where `record.Date.Date >= startDate.Date && record.Date.Date <= endDate.Date`.
- **BR-4.2.5**: Records within the range are sorted by date (ascending).
- **BR-4.2.6**: If no records exist in the date range, the report displays "No attendance records found for this period."

### 4.3 PDF Report Content Per Service
- **BR-4.3.1**: Each attendance record generates a section with: service type label, date, total present count.
- **BR-4.3.2**: An attendees table lists: row number, name, age group, and category, sorted alphabetically by name.
- **BR-4.3.3**: First timers are highlighted with a yellow background (`Colors.Yellow.Lighten4`) and an asterisk (`*`) appended to their name.
- **BR-4.3.4**: Non-first-timer rows have a white background.
- **BR-4.3.5**: Below the attendees table, age group counts and category counts are displayed in a side-by-side layout.
- **BR-4.3.6**: If first timers exist, a "* First Timers:" section lists their names.
- **BR-4.3.7**: Each section is separated by a horizontal line.

### 4.4 PDF Report Footer
- **BR-4.4.1**: Every page has a centered footer showing "Page {current} of {total}".

### 4.5 PDF Download
- **BR-4.5.1**: From the Reports page, the file name is: `Church-Attendance-Report-{startDate}-to-{endDate}.pdf`.
- **BR-4.5.2**: From the Attendance page (single date), the file name is: `Church-Attendance-{ServiceLabel}-{date}.pdf` where ServiceLabel has spaces replaced with hyphens.
- **BR-4.5.3**: The Content-Type is `application/pdf` with a `Content-Disposition: attachment` header.

### 4.6 PDF Sharing (Web Share API)
- **BR-4.6.1**: On mobile devices, the "Download PDF" button intercepts the click and uses the Web Share API to share the PDF file instead.
- **BR-4.6.2**: Mobile detection uses user agent regex: `/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i`.
- **BR-4.6.3**: A "Share PDF" button is always visible and calls the Web Share API.
- **BR-4.6.4**: If the Web Share API is not supported, an error message is shown: "Sharing is not supported on this device. Use 'Download PDF' instead."
- **BR-4.6.5**: If file sharing is not supported by the browser, the system falls back to form submission for download.
- **BR-4.6.6**: If the user cancels the share dialog (`AbortError`), the status is silently cleared.
- **BR-4.6.7**: On the Attendance page, a "Share PDF" button is also available, which shares a single-date PDF.

### 4.7 Report Date Input Synchronization
- **BR-4.7.1**: The Reports page has visible date inputs and hidden form fields (with classes `.report-start-date` and `.report-end-date`) in the export and email forms.
- **BR-4.7.2**: When the user changes a visible date input, all corresponding hidden fields are synchronized via JavaScript `input` event listener.

## 5. Email

### 5.1 Email Delivery
- **BR-5.1.1**: Emails are sent using MailKit's SMTP client.
- **BR-5.1.2**: The "From" address is labeled "Church Attendance" with the configured `FromEmail`.
- **BR-5.1.3**: The "To" address uses the configured `ToEmail`.
- **BR-5.1.4**: The email subject is: "Attendance Report - {startDate}-to-{endDate}".
- **BR-5.1.5**: The email body text is: "Please find the attendance report ({reportName}) attached."
- **BR-5.1.6**: The PDF is attached with filename: `attendance-report-{reportName}.pdf`.

### 5.2 Email Prerequisites
- **BR-5.2.1**: SMTP settings must be configured before sending email. If the `Host` field is empty, the system returns an error: "Please configure SMTP settings first."
- **BR-5.2.2**: Both start and end dates must be valid. If either is invalid, an error message "Invalid date range" is returned.

### 5.3 Email Error Handling
- **BR-5.3.1**: If the email sends successfully, a success message "Email sent successfully!" is displayed.
- **BR-5.3.2**: If the email fails, an error message "Failed to send email: {exception message}" is displayed.

## 6. Settings (SMTP Configuration)

### 6.1 SMTP Settings Data Model
- **BR-6.1.1**: SMTP settings include: `Host` (string), `Port` (int), `Username` (string), `Password` (string), `FromEmail` (string), `ToEmail` (string), `UseSsl` (boolean).
- **BR-6.1.2**: Default values: Host="", Port=587, Username="", Password="", FromEmail="", ToEmail="", UseSsl=true.

### 6.2 SMTP Settings Form
- **BR-6.2.1**: All fields are marked as `required` in the HTML form.
- **BR-6.2.2**: The Host field placeholder is "smtp.gmail.com".
- **BR-6.2.3**: The `UseSsl` checkbox maps to `value="true"`. When checked, `UseSsl = true`. When unchecked, the form does not submit the field, so `UseSsl = false` (because `formValue ctx "useSsl"` returns `None`, and `None = Some "true"` is `false`).

### 6.3 SMTP Connection
- **BR-6.3.1**: If `UseSsl` is true, the connection uses `SecureSocketOptions.StartTls`.
- **BR-6.3.2**: If `UseSsl` is false, the connection uses `SecureSocketOptions.None`.
- **BR-6.3.3**: Authentication is optional: credentials are only sent if both `Username` and `Password` are non-empty.

### 6.4 SMTP Settings Persistence
- **BR-6.4.1**: SMTP settings are saved to `settings.json` in the data directory.
- **BR-6.4.2**: On successful save, the message "SMTP settings saved!" is displayed.
- **BR-6.4.3**: If the Port value is not a valid integer, it defaults to 587.

## 7. Data Persistence

### 7.1 Storage Location
- **BR-7.1.1**: All data files are stored in a `data/` directory under `AppContext.BaseDirectory`.
- **BR-7.1.2**: The data directory is automatically created on application startup if it does not exist.

### 7.2 Data Files
- **BR-7.2.1**: Members are stored in `data/members.json` as a JSON array.
- **BR-7.2.2**: Attendance records are stored in `data/attendance.json` as a JSON array.
- **BR-7.2.3**: SMTP settings are stored in `data/settings.json` as a JSON object.

### 7.3 File Read Behavior
- **BR-7.3.1**: If a data file does not exist, the default value is returned (empty list for members/attendance, default SMTP settings for settings).
- **BR-7.3.2**: If a data file exists but is empty or whitespace-only, the default value is returned.
- **BR-7.3.3**: JSON deserialization uses `WriteIndented = true` for human-readable formatting.

### 7.4 Thread Safety
- **BR-7.4.1**: All file read and write operations are protected by a shared lock object (`lockObj`).
- **BR-7.4.2**: The lock is acquired using F#'s `lock` function, ensuring mutual exclusion across concurrent requests.

### 7.5 JSON Serialization
- **BR-7.5.1**: F# discriminated unions are serialized using `JsonUnionEncoding.UnwrapFieldlessTags` combined with `JsonUnionEncoding.Default`, meaning fieldless union cases (like `Men`, `SundayService`) are serialized as simple strings rather than `{"Case":"Value"}` objects.
- **BR-7.5.2**: The JSON serializer uses `System.Text.Json` with `FSharp.SystemTextJson` converters.

## 8. UI Behavior and Navigation

### 8.1 Navigation
- **BR-8.1.1**: The application has five main pages: Dashboard (`/`), Members (`/members`), Attendance (`/attendance`), Reports (`/reports`), Settings (`/settings`).
- **BR-8.1.2**: The active navigation item is visually indicated with bold text and underline.

### 8.2 HTMX Interactions
- **BR-8.2.1**: The application uses HTMX for partial page updates without full reloads.
- **BR-8.2.2**: Member creation and editing use HTMX POST/PUT to update the members table and close the modal via injected `<script>` tags.
- **BR-8.2.3**: Member deactivation uses HTMX DELETE to update the members table.
- **BR-8.2.4**: The attendance checklist is loaded via HTMX GET on `load` and on date `change` events.
- **BR-8.2.5**: Attendance form submission (View Summary) uses HTMX POST and replaces the attendance area.
- **BR-8.2.6**: SMTP settings form uses HTMX POST with response rendered in a status div.
- **BR-8.2.7**: Email report uses HTMX POST with response rendered in a status div.
- **BR-8.2.8**: The HTMX request is identified by checking for the `HX-Request` header.

### 8.3 Modal System
- **BR-8.3.1**: Members are created and edited in a modal overlay (`#member-modal-overlay`).
- **BR-8.3.2**: The modal is opened by setting `overlay.style.display = 'flex'` and body overflow to `hidden`.
- **BR-8.3.3**: The modal is closed by setting `overlay.style.display = 'none'` and restoring body overflow.
- **BR-8.3.4**: The modal can be closed by: clicking the X button, clicking the Cancel button, clicking outside the modal content area (on the overlay), or via server response after successful save (injected script).
- **BR-8.3.5**: The modal title is dynamically set: "Add New Member" for creation, "Edit Member" for editing.
- **BR-8.3.6**: The modal form content is loaded via HTMX into `#member-form-area`.
- **BR-8.3.7**: When creating: the form uses `hx-post="/members"`. When editing: the form uses `hx-put="/members/{id}"`.
- **BR-8.3.8**: Both create and edit forms target `#members-table` with `outerHTML` swap.

### 8.4 Form Validation
- **BR-8.4.1**: The Full Name field has `required` attribute.
- **BR-8.4.2**: The Age Group and Category dropdowns have `required` attribute.
- **BR-8.4.3**: The First Attended Date field is optional (no `required` attribute).
- **BR-8.4.4**: The date input on the attendance page has `required` attribute.

### 8.5 HTML Encoding
- **BR-8.5.1**: User-provided values (member names, dates, etc.) are HTML-encoded via `System.Net.WebUtility.HtmlEncode` to prevent XSS attacks when rendered in templates.

## 9. Server Configuration

### 9.1 Network
- **BR-9.1.1**: The application listens on port 5050 (HTTP) and port 5051 (HTTPS).
- **BR-9.1.2**: Both endpoints support HTTP/1.1 and HTTP/2 protocols.
- **BR-9.1.3**: HTTPS uses a PFX certificate located at `{AppContext.BaseDirectory}/certs/certificate.pfx`.
- **BR-9.1.4**: The application binds to all network interfaces (`ListenAnyIP`).

### 9.2 Static Files
- **BR-9.2.1**: Static files are served from the `wwwroot/` directory (ASP.NET convention via `UseStaticFiles()`).
- **BR-9.2.2**: Static assets include: `/js/app.js` (client-side JavaScript) and `/css/app.css` (custom styles).
- **BR-9.2.3**: External dependencies are loaded from CDN: Pico CSS v2 and HTMX v2.0.4.
- **BR-9.2.4**: The CSS file includes a cache-busting query parameter (`?v=4`).

### 9.3 Routing
- **BR-9.3.1**: Routes use ASP.NET Minimal APIs with `RequestDelegate` wrappers.
- **BR-9.3.2**: Route parameters (e.g., `{id}`) are extracted via `ctx.GetRouteValue("id") :?> string`.
- **BR-9.3.3**: All responses set `Content-Type: text/html; charset=utf-8` except PDF downloads which use `application/pdf`.

## 10. CSS and Visual Design

### 10.1 Framework
- **BR-10.1.1**: The application uses Pico CSS v2 as the base styling framework with the `light` theme.
- **BR-10.1.2**: Custom styles extend Pico CSS in `/css/app.css`.

### 10.2 Status Messages
- **BR-10.2.1**: Success messages have green background (`#d4edda`), dark green text (`#155724`), and green border.
- **BR-10.2.2**: Error messages have red/pink background (`#f8d7da`), dark red text (`#721c24`), and pink border.

### 10.3 Responsive Design
- **BR-10.3.1**: The modal adjusts for mobile screens at breakpoint 768px (max-width reduced, padding adjusted).
- **BR-10.3.2**: The attendance counter font size is reduced on mobile.
