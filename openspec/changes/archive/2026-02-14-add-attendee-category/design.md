## Context

The app uses an F# discriminated union `Category` with three cases: `Member`, `Visitor`, `UnderMonitoring`. This type flows through Domain.fs (model + helpers), Templates.fs (HTML rendering), Handlers.fs (form parsing), and PdfService.fs (PDF reports). Adding a new case is mechanical — extend the DU and update each pattern match.

Data is stored as JSON flat files. The `Category` field serializes as a plain string via `JsonUnionEncoding.UnwrapFieldlessTags`. Adding `Attendee` requires no schema migration — existing records keep their current category values, and new values are immediately valid.

## Goals / Non-Goals

**Goals:**
- Add `Attendee` as a valid category throughout the system
- Maintain category ordering: Member → Attendee → UnderMonitoring → Visitor

**Non-Goals:**
- Migrating existing people from Member → Attendee (separate change)
- Changing the `Member` record type name (acknowledged naming quirk, not addressing now)

## Decisions

**1. DU case positioning: after Member**
Place `Attendee` immediately after `Member` in the DU definition. The `allCategories` list defines display order explicitly, so DU order is cosmetic, but keeping them aligned avoids confusion.

**2. Serialized value: `"Attendee"`**
With `UnwrapFieldlessTags`, the case name serializes directly. No custom serialization needed. `parseCategory` will accept `"Attendee"` as input.

**3. No default change**
New member form defaults remain `Member`. Attendee is an explicit choice, not a default.

## Risks / Trade-offs

- **[Exhaustiveness]** F# compiler will flag every incomplete pattern match on `Category` once `Attendee` is added → This is a feature, not a risk. Compiler guides us to every touchpoint.
- **[Data files]** Existing JSON files have no `"Attendee"` values yet → No risk. Old data is unaffected. New values work immediately.
