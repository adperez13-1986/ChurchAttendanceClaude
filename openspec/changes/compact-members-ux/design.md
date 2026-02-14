## Context

The Members page currently renders all members in a flat `<table>` with columns: Name, Age Group, Category, Status, Actions. With ~179 members, scrolling is excessive. The attendance page already has collapsible age group sections — the same CSS classes (`.age-group-section`, `.age-group-header`, `.collapsed`, `.section-toggle`, `.age-group-body`) and JS toggle logic can be reused.

The members table has HTMX interactions: Edit buttons load a form into a modal, Deactivate buttons delete via `hx-delete` and re-render the `#members-table` target. The table is also re-rendered after add/edit/deactivate operations, which resets the DOM.

## Goals / Non-Goals

**Goals:**
- Reduce scrolling on the Members page by grouping into collapsible age group sections
- Reuse the collapsible section pattern from the attendance page
- Maintain all existing member management functionality (edit, deactivate, add, search)

**Non-Goals:**
- Changing the member table columns or row layout
- Changing the Add New Member modal flow
- Adding sticky top/bottom bars (not needed — the page isn't a ticking workflow)

## Decisions

### 1. Wrap each age group in a collapsible section containing a sub-table

**Decision:** Split the single `<table id="members-table">` into multiple tables, one per age group, each wrapped in a `.age-group-section` div. The Age Group column is dropped from the table since the section header shows it.

**Rationale:** Reuses existing CSS. Each section header shows the group name and count. The table within each section keeps Name, Category, Status, Actions columns.

**HTMX impact:** The current `hx-target="#members-table"` on edit/deactivate/add targets a single table. With multiple tables, the target needs to change to a wrapper div (e.g., `#members-sections`) that contains all sections. The server re-renders all sections into this wrapper on each HTMX response.

### 2. Keep name search always visible, remove age group dropdown

**Decision:** The name search input stays at the top of the page. The age group dropdown filter is removed — sections replace it. Search filters across all sections, hiding rows that don't match and hiding sections with zero visible rows.

**Rationale:** Finding a member to edit is a frequent action. Search should be immediately accessible, not hidden behind an icon.

### 3. Reuse CSS classes from attendance

**Decision:** Use the same `.age-group-section`, `.age-group-header`, `.collapsed`, `.section-toggle`, `.age-group-body` CSS classes. The existing collapsible click handler in app.js already works with these classes via event delegation — no new JS needed for toggle behavior.

**Rationale:** Consistency across pages. No duplicate CSS.

### 4. Re-collapse after HTMX table refresh

**Decision:** After an HTMX refresh (add/edit/deactivate), all sections re-render from the server in their default collapsed state. This is acceptable because the user just completed an action and the page is resetting.

**Alternative considered:** Preserving expand/collapse state across refreshes via localStorage. Rejected as unnecessary complexity — the user just did one action and can re-expand.

## Risks / Trade-offs

- **[HTMX target change]** → Changing from `#members-table` to `#members-sections` requires updating the `membersTable` function and the HTMX attributes on edit/deactivate/add buttons. Low risk, straightforward.
- **[Search + collapsed sections]** → When searching, collapsed sections with matching members need to expand to show results. The filter function should temporarily show all rows/sections that match, regardless of collapsed state. → Handled by showing matching rows and their parent sections.
