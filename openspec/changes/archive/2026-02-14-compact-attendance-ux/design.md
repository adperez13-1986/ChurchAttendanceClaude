## Context

The attendance checklist page (`attendanceChecklist` in Templates.fs) currently renders a full table with 4 columns (checkbox, name, age group, category), preceded by a large date banner, attendance counter block, and two filter inputs. On a phone, this pushes the first checkbox ~420px below the viewport top, requiring significant scrolling. The ushers' paper-based workflow uses separate printed sheets per age group with just names and tick boxes — simple and fast.

The page is rendered as an HTMX partial into `#attendance-area`. Auto-save fires on every checkbox toggle via JS fetch to `/attendance/auto-save`. The form posts to `/attendance` for the summary view. PDF sharing uses the Web Share API via `shareAttendancePdf()`.

Age groups: Men, Women, YAN, CYN, Children, Infants (6 groups from Domain.fs).

## Goals / Non-Goals

**Goals:**
- Reduce vertical space above the first checkbox to ~90px (from ~420px)
- Match the paper experience: names and checkboxes, grouped by age group
- Make Share PDF and View Summary accessible without scrolling to the bottom
- Keep auto-save, past-date confirmation, and PDF sharing behaviour unchanged

**Non-Goals:**
- Changing the Members page layout or filters (those remain as-is)
- Changing the attendance summary page
- Changing backend API routes or data model
- Adding new features (e.g., multi-user concurrent attendance)

## Decisions

### 1. Replace table with flat list grouped by age group sections

**Decision:** Render members as a flat `<label>` list (checkbox + name) instead of a `<table>` with columns. Group members under age group section headers.

**Rationale:** The table's Age Group and Category columns aren't useful during ticking. A flat list is more compact and matches the paper experience. Section headers replace the age group dropdown filter — the visual grouping *is* the filter.

**Alternative considered:** Keep the table but hide columns via CSS on mobile. Rejected because the table semantics add unnecessary DOM complexity and the columns aren't needed on any form factor during ticking.

**Template structure:**
```
<div class="age-group-section">
  <div class="age-group-header">Men (5/12)</div>
  <label class="attendance-row" data-name="alice smith">
    <input type="checkbox" name="memberIds" value="{id}"> Alice Smith
  </label>
  ...
</div>
```

Members are sorted alphabetically within each age group. Age groups are rendered in the order from `Domain.allAgeGroups` (Men, Women, YAN, CYN, Children, Infants).

### 2. Sticky top bar with compact info

**Decision:** Replace the date banner + attendance counter + filter section with a single sticky `<div>` containing: abbreviated service label, date, total present count, and a search icon.

**Rationale:** Condenses ~330px of chrome into ~40px. The count updates live as checkboxes toggle (same JS logic, different target element). The service label is abbreviated (e.g., "Sun Service" not the full banner).

**CSS approach:** `position: sticky; top: 0; z-index: 10;` on the bar. Since the checklist is rendered inside `<main class="container">`, the sticky bar sits within the container flow.

### 3. Expandable search behind icon

**Decision:** The top bar has a search icon button. Tapping it shows a search input that replaces/overlays the top bar content. Typing filters all visible names across all sections (same matching logic as current `filterAttendanceRows`). Clearing or tapping X returns to the normal top bar.

**Rationale:** Search is a secondary action for 65 members. Making it always-visible wastes space. But it's valuable for the "late arrival" scenario, so it should be one tap away.

**Implementation:** Toggle a CSS class on the top bar (e.g., `.searching`) that hides the info text and shows the search input. The search input gets `autofocus` when shown. Rows that don't match are hidden via `display: none` (same as current approach). Section headers with zero visible members should also hide.

### 4. Sticky bottom bar with action buttons

**Decision:** Move View Summary and Share PDF buttons to a fixed/sticky bar at the bottom of the viewport. Include auto-save status indicator.

**Rationale:** These are always needed but currently require scrolling past all 65 members. A sticky footer makes them instantly accessible.

**CSS approach:** `position: sticky; bottom: 0;` with a background colour and subtle top border/shadow so content scrolls behind it cleanly.

### 5. Per-section counts in headers

**Decision:** Each age group header shows "{checked}/{total}" count (e.g., "Men (5/12)"). The sticky top bar shows total present across all groups. Both update live on checkbox changes.

**Rationale:** Replaces the old monolithic counter with distributed, contextual counts. The usher can see at a glance how many are present in each group.

**JS approach:** Update the existing `updateAttendanceCount()` function to iterate through sections and update each header's count, plus the top bar total.

### 6. Past-date confirmation stays as-is

**Decision:** Keep the current confirmation dialog for past dates. It appears before the checklist (same as today). Once confirmed, the compact checklist renders.

**Rationale:** No UX change needed here — it's a one-time gate, not a recurring interaction.

## Risks / Trade-offs

- **[Lost information during ticking]** → Age Group and Category columns are removed from the checklist. Acceptable because section headers show age group, and category isn't needed during ticking. Both remain visible on the Members page and in reports.
- **[Search discoverability]** → Hidden behind an icon, new users might not find it. → Mitigated by the fact that with only 65 members in sections, search is rarely needed. The icon is standard and recognisable.
- **[Sticky positioning on older mobile browsers]** → `position: sticky` is well-supported (95%+ on caniuse). No polyfill needed.
- **[Select-all removed]** → The current select-all checkbox is removed. If needed for a "everyone attended" scenario, the usher must tick individually. Acceptable trade-off for the simpler UI. Could be re-added later in the top bar if needed.
