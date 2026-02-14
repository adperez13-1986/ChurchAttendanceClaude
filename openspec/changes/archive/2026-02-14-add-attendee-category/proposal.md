## Why

The current category system (Member, Visitor, UnderMonitoring) has no option for people who attend regularly but don't want official membership. These regulars are currently miscategorized as either Member (overstates status) or Visitor (understates involvement).

## What Changes

- Add `Attendee` to the `Category` discriminated union — positioned between Member and Visitor
- New category order: Member → Attendee → UnderMonitoring → Visitor
- `Attendee` appears in all dropdowns, tables, reports, and PDF exports where categories are shown
- Default category for new registrations remains `Member`

## Capabilities

### New Capabilities

- `attendee-category`: A new person category for regular but non-official attendees, integrated into forms, tables, attendance checklists, and PDF reports

### Modified Capabilities

_None — no existing spec-level requirements change. This is purely additive._

## Impact

- `Domain.fs`: Category DU, categoryLabel, parseCategory, allCategories
- `Templates.fs`: Form dropdowns, member table rows, attendance checklist rows
- `Handlers.fs`: Category parsing from form submissions
- `PdfService.fs`: Category column in PDF reports, summary counts by category
- `data/*.json`: Existing data files unaffected (migration handled separately)
