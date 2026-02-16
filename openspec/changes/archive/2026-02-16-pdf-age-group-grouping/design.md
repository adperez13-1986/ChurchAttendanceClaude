## Context

The PDF report (used by both attendance "Share PDF" and reports "Download PDF") renders a single flat table of attendees sorted alphabetically. The attendance checklist UI already groups members by age group with collapsible sections. Users expect the PDF to follow the same grouping pattern.

## Goals / Non-Goals

**Goals:**
- Group PDF attendees by age group (Men, Women, YAN, CYN, Children, Infants)
- Show a section header with group name and count per group
- Number members within each group (restarting at 1 per group)
- Remove the now-redundant Age Group column from the table

**Non-Goals:**
- Changing the summary tables (age group counts, category counts) — they already exist
- Changing the first timers section
- Creating a separate PDF layout for attendance vs. reports — they share `generateReport`

## Decisions

**Replace flat table loop with per-age-group loop**: Instead of sorting all attendees alphabetically and rendering one table, iterate over `Domain.allAgeGroups`, filter attendees per group, and render a separate headed table for each non-empty group. This mirrors the `compact-attendance-checklist` UI pattern.

**Remove Age Group column**: Since the section header identifies the group, the column is redundant. Table columns become: #, Name, Category (3 columns instead of 4).

**Numbering restarts per group**: Each group's table starts numbering at 1. This is more natural when scanning a grouped layout.

## Risks / Trade-offs

- **Page breaks**: Groups may split across pages. QuestPDF handles this automatically with its layout engine, so no special handling needed.
- **Shared function**: Both attendance and reports pages use `generateReport`. This change affects both — which is the desired behavior since consistency matters.
