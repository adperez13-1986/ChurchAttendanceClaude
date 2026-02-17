## ADDED Requirements

### Requirement: App uses a brand accent color
The app SHALL use teal (#0d9488) as the primary accent color, applied via Pico CSS custom property overrides. Buttons, links, active nav indicators, and form focus rings SHALL use this color.

#### Scenario: Accent color is applied consistently
- **WHEN** a user views any page
- **THEN** interactive elements (buttons, links, active nav indicator) use the teal accent color

### Requirement: Dark mode toggle in navigation
The app SHALL display a dark mode toggle button in the navigation bar. The toggle SHALL switch between light and dark themes by changing the `data-theme` attribute on the `<html>` element.

#### Scenario: Toggle to dark mode
- **WHEN** a user clicks the dark mode toggle while in light mode
- **THEN** the page switches to dark mode and the toggle icon updates to indicate the current theme

#### Scenario: Toggle to light mode
- **WHEN** a user clicks the dark mode toggle while in dark mode
- **THEN** the page switches to light mode and the toggle icon updates to indicate the current theme

### Requirement: Theme preference persists across sessions
The app SHALL store the user's theme choice in `localStorage`. On page load, the app SHALL apply the stored theme preference before rendering.

#### Scenario: Theme persists after refresh
- **WHEN** a user selects dark mode and refreshes the page
- **THEN** the page loads in dark mode

#### Scenario: No stored preference
- **WHEN** no theme preference is stored in localStorage
- **THEN** the app defaults to light mode

### Requirement: Active nav link uses colored bottom border
The active navigation link SHALL be indicated by a colored bottom border using the brand accent color, instead of a text underline.

#### Scenario: Active nav indicator style
- **WHEN** a user views any page
- **THEN** the active nav link has a colored bottom border and no text underline

### Requirement: Cards have subtle shadows and rounded corners
Article/card elements SHALL have a subtle box-shadow and rounded border-radius for visual depth.

#### Scenario: Card styling is applied
- **WHEN** a user views a page with article/card elements
- **THEN** cards have a subtle shadow and rounded corners

### Requirement: Custom styles adapt to dark mode
All custom CSS styles (status messages, attendance bars, age group headers, etc.) SHALL use Pico CSS variables instead of hardcoded colors, ensuring they adapt correctly in dark mode.

#### Scenario: Custom elements render correctly in dark mode
- **WHEN** a user views the attendance page in dark mode
- **THEN** the sticky top bar, age group headers, attendance rows, and status messages use appropriate dark mode colors
