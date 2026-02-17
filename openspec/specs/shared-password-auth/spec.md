### Requirement: Unauthenticated requests are redirected to login
The system SHALL redirect any unauthenticated request to `GET /login`, except for static files served from `wwwroot/`.

#### Scenario: Unauthenticated user visits a protected page
- **WHEN** a user without a valid auth cookie requests any app route (e.g., `/`, `/members`, `/attendance`)
- **THEN** the system redirects them to `/login`

#### Scenario: Static files are accessible without authentication
- **WHEN** a user without a valid auth cookie requests a static file (e.g., `/css/app.css`, `/js/app.js`)
- **THEN** the file is served normally without redirect

### Requirement: Login page accepts a shared password
The system SHALL display a login page at `GET /login` with a single password field and a submit button. The page SHALL use Pico CSS styling but SHALL NOT include the app's navigation bar.

#### Scenario: Login page is displayed
- **WHEN** a user visits `/login`
- **THEN** the system displays a centered form with a password input and a "Login" button

#### Scenario: Correct password submitted
- **WHEN** a user submits the correct password via `POST /login`
- **THEN** the system sets a persistent authentication cookie (7-day expiration) and redirects to `/`

#### Scenario: Incorrect password submitted
- **WHEN** a user submits an incorrect password via `POST /login`
- **THEN** the system re-displays the login page with an error message "Invalid password"

### Requirement: Password is configured via environment variable
The system SHALL read the shared password from the `APP_PASSWORD` environment variable. If the variable is not set, the system SHALL fall back to `"changeme"`.

#### Scenario: Environment variable is set
- **WHEN** `APP_PASSWORD` is set to `"mySecurePass123"`
- **THEN** the system accepts `"mySecurePass123"` as the valid password

#### Scenario: Environment variable is not set
- **WHEN** `APP_PASSWORD` is not set
- **THEN** the system accepts `"changeme"` as the valid password and logs a warning at startup

### Requirement: Logout clears the session
The system SHALL provide a logout mechanism at `POST /logout` that clears the authentication cookie and redirects to `/login`.

#### Scenario: User logs out
- **WHEN** an authenticated user submits `POST /logout`
- **THEN** the authentication cookie is cleared and the user is redirected to `/login`

### Requirement: Navigation bar includes logout
The system SHALL display a "Logout" button in the navigation bar on all authenticated pages.

#### Scenario: Logout button is visible
- **WHEN** an authenticated user views any page
- **THEN** the navigation bar includes a "Logout" button that submits `POST /logout`
