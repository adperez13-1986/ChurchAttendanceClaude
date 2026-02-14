## 1. Template Changes

- [x] 1.1 Add `data-age-group` and `data-name` attributes to `<tr>` elements in `memberRow` (Templates.fs)
- [x] 1.2 Add age group filter dropdown and name search input to `membersPage`, placed between the "Add New Member" button and the table (Templates.fs)

## 2. Client-Side Filtering

- [x] 2.1 Add `filterMemberRows` function in app.js that filters member table rows by `data-age-group` and `data-name` attributes
- [x] 2.2 Wire up event listeners for the members page age group dropdown (`change`) and name search input (`input`)

## 3. HTMX Filter Reset

- [x] 3.1 Reset age group dropdown and name search input to defaults when members table is swapped via HTMX (`htmx:afterSwap` or `htmx:afterOnLoad`)
