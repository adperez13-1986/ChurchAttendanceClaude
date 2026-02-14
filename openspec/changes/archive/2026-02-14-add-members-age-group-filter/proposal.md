## Why

The attendance page has an age group filter and name search that make it easy to find specific members in the checklist. The members page has no such filtering, making it harder to locate members as the list grows. Adding the same filtering to the members page creates a consistent UX across both pages.

## What Changes

- Add an age group dropdown filter to the members page, matching the attendance page pattern
- Add a name search input to the members page
- Add `data-age-group` and `data-name` attributes to member table rows for client-side filtering
- Preserve or reset filter state when the members table is refreshed via HTMX (add/edit/deactivate)

## Capabilities

### New Capabilities
- `member-list-filtering`: Client-side filtering of the members table by age group and name search

### Modified Capabilities

## Impact

- `Templates.fs`: `membersPage` and `memberRow` functions modified to include filter UI and data attributes
- `app.js`: New filtering logic for the members page (or generalization of the existing attendance filter)
