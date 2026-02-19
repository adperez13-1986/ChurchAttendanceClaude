## ADDED Requirements

### Requirement: Tenant configuration file
The system SHALL read tenant configuration from `~/.church-attendance/tenants.json` at startup. The file SHALL contain a `default` field (tenant slug for fallback), a `domain` field (the base domain for subdomain routing), and a `tenants` object mapping slugs to tenant config (name and password).

#### Scenario: Valid tenants.json exists
- **WHEN** the application starts and `~/.church-attendance/tenants.json` exists with valid JSON
- **THEN** the system SHALL load all tenant configurations and use them for routing and authentication

#### Scenario: tenants.json does not exist
- **WHEN** the application starts and `~/.church-attendance/tenants.json` does not exist
- **THEN** the system SHALL create a default `tenants.json` with a single tenant (slug: `"default"`, name: `"Church Attendance"`, password from `APP_PASSWORD` env var or `"changeme"`) and `default` set to `"default"`

#### Scenario: tenants.json structure
- **WHEN** the system reads `tenants.json`
- **THEN** it SHALL expect the following structure:
  ```json
  {
    "default": "<slug>",
    "domain": "<base-domain>",
    "tenants": {
      "<slug>": { "name": "<display name>", "password": "<password>" }
    }
  }
  ```

### Requirement: Subdomain-based tenant routing
The system SHALL extract the tenant from the `Host` request header by matching the first subdomain label against configured tenant slugs.

#### Scenario: Request with known subdomain
- **WHEN** a request arrives with `Host: vienna.jilaustria.org`
- **AND** `vienna` is a configured tenant slug
- **THEN** the system SHALL route the request to tenant `vienna`

#### Scenario: Request with unknown subdomain
- **WHEN** a request arrives with a subdomain not matching any configured tenant
- **THEN** the system SHALL fall back to the default tenant

#### Scenario: Request with no subdomain (bare domain)
- **WHEN** a request arrives with `Host: jilaustria.org`
- **AND** the host matches the configured `domain` exactly
- **THEN** the system SHALL serve the landing page instead of any tenant

#### Scenario: Request from localhost
- **WHEN** a request arrives with `Host: localhost:5050` (no subdomain, doesn't match configured domain)
- **THEN** the system SHALL use the default tenant

### Requirement: Tenant context available to handlers
The system SHALL store the resolved tenant slug in `HttpContext.Items["Tenant"]` so that all handlers can access it.

#### Scenario: Handler accesses tenant
- **WHEN** a handler processes a request that resolved to tenant `vienna`
- **THEN** `ctx.Items["Tenant"]` SHALL equal `"vienna"`

### Requirement: Landing page at bare domain
The system SHALL serve a landing page at the bare domain (e.g., `jilaustria.org`) that displays links to each configured tenant's subdomain.

#### Scenario: User visits bare domain
- **WHEN** a user visits `jilaustria.org`
- **THEN** the system SHALL display a page listing all configured tenants with links to their subdomains (e.g., `vienna.jilaustria.org`, `donaustadt.jilaustria.org`)
- **AND** the page SHALL NOT require authentication

### Requirement: Church name displayed in app
The system SHALL display the tenant's configured name in the application header/title on all authenticated pages.

#### Scenario: Authenticated page shows church name
- **WHEN** an authenticated user views any page for tenant `vienna` (configured name: "JIL Vienna")
- **THEN** the page title and header SHALL include "JIL Vienna"
