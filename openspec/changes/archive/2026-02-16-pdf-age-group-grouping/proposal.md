## Why

The PDF attendance report lists all attendees in a single flat alphabetical table, making it hard to scan by age group. The attendance checklist UI already groups by age group — the PDF should match.

## What Changes

- Replace the flat attendees table in the PDF with per-age-group sections
- Each section gets a bold header with group name and count (e.g., "Men (12)")
- Members within each section are numbered and sorted alphabetically
- The Age Group column is removed from the table since the section header provides it
- Columns per section: #, Name, Category
- Empty age groups are omitted
- First timer highlighting (yellow background + asterisk) is preserved within each section

## Capabilities

### New Capabilities

_None — this modifies existing PDF generation behavior._

### Modified Capabilities

_None — no spec-level requirements change. The PDF still shows the same data (attendees, counts, first timers). This is a layout/presentation change within the existing report generation capability._

## Impact

- `PdfService.fs`: Attendees table rendering replaced with grouped sections
- Affects both attendance page "Share PDF" and reports page "Download PDF" (shared `generateReport` function)
- No API changes, no data model changes
