## 1. Templates.fs — Rewrite attendance checklist markup

- [x] 1.1 Rewrite `attendanceChecklist` function to render members grouped by age group sections instead of a flat table. Each section has a header div with group name and count placeholder. Members rendered as `<label>` with checkbox + name, sorted alphabetically within each group. Skip empty age groups.
- [x] 1.2 Replace the date banner + attendance counter + filter inputs with a sticky top bar div containing: abbreviated service label, date, total present count, and a search icon button. Include a hidden search input that toggles visible on icon click.
- [x] 1.3 Replace the bottom action grid (auto-save status + View Summary + Share PDF) with a sticky bottom bar div containing the same elements.
- [x] 1.4 Keep hidden inputs (date, serviceType) and the `hx-post="/attendance"` form wrapping. Keep past-date confirmation logic unchanged.

## 2. app.css — Add compact checklist styles

- [x] 2.1 Add sticky top bar styles: `position: sticky; top: 0; z-index: 10;` with background, padding, and a flex layout for service info + count + search icon.
- [x] 2.2 Add sticky bottom bar styles: `position: sticky; bottom: 0;` with background, top border/shadow, flex layout for buttons and auto-save status.
- [x] 2.3 Add age group section header styles: bold text, subtle background, padding, margin. Include per-section count styling.
- [x] 2.4 Add compact attendance row styles: label with checkbox, appropriate padding, border-bottom for visual separation.
- [x] 2.5 Add expandable search styles: `.searching` class on top bar that hides info and shows search input. Transition/animation optional.
- [x] 2.6 Remove or keep (but unused) the old `.date-banner`, `.attendance-counter` styles — they're still used on other pages if any. Verify and clean up if only used by attendance checklist.

## 3. app.js — Update attendance JS logic

- [x] 3.1 Update `updateAttendanceCount()` to iterate through age group sections, update each section header's count (`{checked}/{total}`), and update the sticky top bar total count.
- [x] 3.2 Add search toggle logic: click handler on search icon shows input + hides info bar (add `.searching` class); close button restores normal bar. Search input triggers `filterAttendanceRows()`.
- [x] 3.3 Update `filterAttendanceRows()` to work with the new DOM structure (labels in sections instead of table rows). Hide sections where all members are filtered out.
- [x] 3.4 Update `autoSaveAttendance()` selectors if form structure changed (verify `form[hx-post="/attendance"]` still works).
- [x] 3.5 Remove select-all checkbox logic from the attendance page event listeners (keep members page logic intact).
- [x] 3.6 Verify `shareAttendancePdf()` still works with the new DOM (it references `form[hx-post="/attendance"]` and hidden inputs — should be unaffected).

## 4. Verification

- [x] 4.1 Build and run the app. Load attendance page, verify checklist renders with age group sections, sticky top/bottom bars, and compact name rows.
- [ ] 4.2 Test checkbox toggling — verify per-section counts and top bar total update live, and auto-save fires correctly.
- [ ] 4.3 Test search — open via icon, type a name, verify filtering across sections, verify sections with no matches hide, verify close restores all.
- [ ] 4.4 Test Share PDF and View Summary buttons in sticky footer.
- [x] 4.5 Test past-date confirmation flow still works.
- [ ] 4.6 Test on narrow viewport (mobile-width) to verify layout is compact and usable.
