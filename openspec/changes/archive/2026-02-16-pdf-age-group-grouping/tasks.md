## 1. PDF Attendees Table Refactor

- [x] 1.1 Replace flat attendees table in `PdfService.fs` with per-age-group loop over `Domain.allAgeGroups`
- [x] 1.2 Render bold section header with group name and count (e.g., "Men (12)") for each non-empty group
- [x] 1.3 Render a 3-column table (#, Name, Category) per group with numbering restarting at 1
- [x] 1.4 Preserve first timer yellow background and asterisk within grouped rows
- [x] 1.5 Skip empty age groups (no section rendered)

## 2. Verification

- [x] 2.1 Build succeeds with no warnings
- [x] 2.2 Generate a PDF from Attendance page "Share PDF" and verify age group sections appear correctly
- [x] 2.3 Generate a PDF from Reports page "Download PDF" and verify same grouping
