## Context

The attendance page already implements client-side filtering with an age group dropdown and name search. The pattern uses `data-*` attributes on table rows and JavaScript to show/hide rows. The members page currently has no filtering.

The members table is swapped via HTMX (`hx-target="#members-table"`) when members are added, edited, or deactivated. This means the table DOM is replaced on these actions.

## Goals / Non-Goals

**Goals:**
- Add age group filter and name search to the members page
- Follow the same client-side filtering pattern used on the attendance page
- Handle HTMX table swaps gracefully

**Non-Goals:**
- Server-side filtering or pagination
- Generalizing/refactoring the attendance filter code into a shared module (keep it simple, duplicate the pattern)

## Decisions

### 1. Client-side filtering (same as attendance page)
**Decision**: Use `data-age-group` and `data-name` attributes on `<tr>` elements with JS show/hide filtering.
**Rationale**: Proven pattern already in the codebase. Member lists are small enough that client-side filtering is performant. No server round-trips needed.

### 2. Reset filters on HTMX table swap
**Decision**: Reset filters to default (All / empty search) when the members table is swapped by HTMX after add/edit/deactivate.
**Rationale**: Simplest approach. Preserving filter state across swaps adds complexity for minimal benefit — the user just performed an action and likely wants to see the full updated list. The filter UI lives outside `#members-table` so it persists in the DOM; we just reset its values and re-show all rows.

### 3. Filter UI placement
**Decision**: Place the filter controls (search + age group dropdown) in a grid row between the "Add New Member" button and the table, matching the attendance page layout.

## Risks / Trade-offs

- [Filter state lost on table swap] → Acceptable per Decision #2. User can re-filter quickly.
- [Duplicate JS logic with attendance page] → Acceptable per Non-Goals. Two small functions are simpler than a premature abstraction.
