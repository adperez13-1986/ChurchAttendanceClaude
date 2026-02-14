## Context

The Members page `filterMemberRows()` has expand/restore logic that the Attendance page `filterAttendanceRows()` lacks. Both functions live in `app.js` and follow the same pattern — iterate sections, filter rows, hide empty sections.

## Goals / Non-Goals

**Goals:**
- Match the Members page search UX: auto-expand sections with hits, restore on clear

**Non-Goals:**
- Changing the Members page search behavior
- Adding new search features beyond expand/restore parity

## Decisions

**Mirror the Members page logic exactly** — add the same `collapsed` class toggling from `filterMemberRows()` into `filterAttendanceRows()`:
- When searching: expand sections with hits, hide sections without
- When cleared: restore all sections to visible (and collapsed state)

This is a ~5 line change in a single function.
