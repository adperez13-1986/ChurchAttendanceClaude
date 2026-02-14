## Context

The codebase uses two different `System.String` methods for empty-string checks: `IsNullOrEmpty` (3 sites) and `IsNullOrWhiteSpace` (3 sites). The `IsNullOrWhiteSpace` variant is strictly more defensive, catching whitespace-only strings that `IsNullOrEmpty` lets through.

## Goals / Non-Goals

**Goals:**
- Standardize on `String.IsNullOrWhiteSpace` across all validation sites
- Ensure whitespace-only SMTP config values are treated as absent

**Non-Goals:**
- Introducing a shared helper function (only 6 call sites — not worth the abstraction)
- Changing any logic beyond swapping the method name

## Decisions

### Decision 1: Use IsNullOrWhiteSpace everywhere

Replace `IsNullOrEmpty` with `IsNullOrWhiteSpace` at all 3 affected call sites. The existing `IsNullOrWhiteSpace` sites are already correct and remain untouched.

**Rationale:** `IsNullOrWhiteSpace` is a strict superset of `IsNullOrEmpty`. No valid use case in this app would treat `"   "` as a meaningful SMTP host or credential. The swap is safe and requires no logic changes.

## Risks / Trade-offs

- **Risk:** A whitespace-only credential that previously "worked" would now be skipped → **Mitigation:** This would only have "worked" by sending whitespace to the SMTP server, which would fail anyway. No real behavior change.
