## Why

The Members page displays all ~179 members in a single flat table, requiring excessive scrolling. The attendance page was recently redesigned with collapsible age group sections, and the same pattern should be applied to Members for consistency and reduced scrolling.

## What Changes

- **Group** the members table by age group using collapsible sections with headers showing group name and member count (e.g., "Men (37)")
- **Default** all sections to collapsed, showing only headers
- **Remove** the age group dropdown filter (replaced by visible sections)
- **Keep** the name search always visible (finding members to edit is a primary action)
- **Keep** the table columns (Name, Category, Status, Actions) since they're needed for member management
- **Keep** the Add New Member button, modal, and all HTMX edit/deactivate functionality unchanged

## Capabilities

### New Capabilities
- `compact-members-list`: Collapsible age-group sections for the members table with per-group counts

### Modified Capabilities
- `member-list-filtering`: Remove the age group dropdown filter requirement (sections replace it). Keep name search unchanged.

## Impact

- **Templates.fs**: `membersTable` and `membersPage` functions — group rows by age group into collapsible sections
- **app.css**: Reuse existing `.age-group-section`, `.age-group-header`, `.collapsed` styles from attendance (may need minor tweaks for table context)
- **app.js**: Add collapsible toggle for members sections, update `filterMemberRows()` to work with new DOM, remove age group filter listener
- No backend/API changes
- No data model changes
