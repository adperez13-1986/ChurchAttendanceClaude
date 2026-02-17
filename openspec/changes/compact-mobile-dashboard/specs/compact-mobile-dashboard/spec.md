## ADDED Requirements

### Requirement: Compact 2x2 stat grid on mobile
On screens ≤768px, the dashboard stat cards SHALL render in a 2-column, 2-row grid instead of stacking vertically.

#### Scenario: Stats display as 2x2 grid on mobile
- **WHEN** a user views the dashboard on a screen ≤768px wide
- **THEN** the 4 stat cards are arranged in a 2x2 grid layout

#### Scenario: Desktop layout is unchanged
- **WHEN** a user views the dashboard on a screen wider than 768px
- **THEN** the 4 stat cards display in a single row (4 columns)

### Requirement: Compact card styling on mobile
On screens ≤768px, each stat card SHALL have reduced padding and a smaller number font size to minimize vertical space.

#### Scenario: Cards use compact styling on mobile
- **WHEN** a user views the dashboard on a screen ≤768px wide
- **THEN** each stat card shows the number at a reduced size (~1.5rem) and the card has less padding than desktop

### Requirement: Compact card labels on mobile
On screens ≤768px, the stat card label SHALL appear as small text below the number, and the Pico article header SHALL be hidden.

#### Scenario: Labels below numbers on mobile
- **WHEN** a user views the dashboard on a screen ≤768px wide
- **THEN** each card shows a small label below the number instead of a header above it

#### Scenario: Headers visible on desktop
- **WHEN** a user views the dashboard on a screen wider than 768px
- **THEN** each card shows the label in the article header as it does currently

### Requirement: Action buttons visible with minimal scrolling
On mobile, the action buttons ("Take Today's Attendance", "View Reports") SHALL be visible without needing to scroll past the stat cards.

#### Scenario: Action buttons near top of page on mobile
- **WHEN** a user views the dashboard on a screen ≤768px wide
- **THEN** the action buttons are visible within the first screenful of content (below the compact stat grid)
