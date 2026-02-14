## 1. Templates.fs — Group members table by age group sections

- [x] 1.1 Rewrite `membersTable` to render members grouped by age group in collapsible sections. Each section wraps a sub-table (Name, Category, Status, Actions — drop the Age Group column). Sections use `.age-group-section collapsed` classes. The outer wrapper uses `id="members-sections"` instead of `id="members-table"`.
- [x] 1.2 Update `membersPage` to remove the age group dropdown filter. Keep the name search input.
- [x] 1.3 Update `memberRow` to remove the Age Group `<td>` column.
- [x] 1.4 Update all HTMX attributes that target `#members-table` to target `#members-sections` instead (in `memberRow` for edit/deactivate, and in `membersPage` for add).

## 2. app.js — Update member filtering and reset logic

- [x] 2.1 Update `filterMemberRows()` to work with the new sectioned DOM: filter `.age-group-section` rows by name, show matching rows and their parent sections (expanding them if needed), hide sections with no matches.
- [x] 2.2 Remove the `member-age-group-filter` change event listener.
- [x] 2.3 Update the HTMX afterOnLoad filter reset to clear the name search input (no age group filter to reset).

## 3. Verification

- [x] 3.1 Build and run. Verify members page renders with collapsible age group sections, all collapsed by default.
- [ ] 3.2 Test expand/collapse toggle on section headers.
- [ ] 3.3 Test name search filters across sections, hides empty sections.
- [ ] 3.4 Test Edit button opens modal and HTMX refresh re-renders sections correctly.
- [ ] 3.5 Test Add New Member and Deactivate buttons work with new `#members-sections` target.
