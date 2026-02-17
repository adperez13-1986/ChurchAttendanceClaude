## Context

The dashboard renders 4 stat cards using Pico CSS's `.grid` class, which displays them in a single row on desktop but stacks them vertically on mobile. Each card uses a Pico `<article>` with header, large number (2.5rem), and optional subtitle — consuming significant vertical space on mobile.

## Goals / Non-Goals

**Goals:**
- Display all 4 stat cards in a compact 2x2 grid on mobile
- Keep action buttons ("Take Today's Attendance", "View Reports") visible with minimal scrolling
- Preserve the existing desktop layout (4-column row)

**Non-Goals:**
- Redesigning the desktop dashboard
- Changing the Recent Activity table
- Adding new stats or removing existing ones

## Decisions

### Use a custom `.stat-grid` CSS class instead of Pico's `.grid`
Pico's `.grid` stacks to 1 column on mobile with no override. A custom CSS grid gives full control over breakpoints and column layout.

### Keep `<article>` elements but reduce padding on mobile
Rather than switching to plain `<div>` elements (which would lose the card look), keep `<article>` and override Pico's padding in the mobile media query. This preserves the desktop appearance with zero changes.

### Move labels below the number on mobile
Currently the label is in an `<article><header>` which Pico renders with background color and padding. On mobile, use a `<small>` label below the number instead, hiding the `<header>`. This is more compact and fits the 2x2 layout.

## Risks / Trade-offs

- [Pico article header override] Hiding the `<header>` on mobile requires careful CSS specificity. → Use a mobile-only class or media query targeting `.stat-grid article header`.
- [Number readability] Shrinking from 2.5rem to ~1.5rem on small screens. → Still large enough to be the visual focus of each card.
