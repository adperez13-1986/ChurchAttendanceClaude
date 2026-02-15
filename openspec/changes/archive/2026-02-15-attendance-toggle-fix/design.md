## Context

The attendance app uses JSON flat files for persistence. When a checkbox is toggled, the frontend collects **all** checked member IDs from the form and POSTs them to `/attendance/auto-save`. The backend replaces the entire `MemberIds` list for that date/service record. With multiple concurrent users, the last write wins and earlier ticks are lost.

The lock in `Database.fs` only protects individual `readFile`/`writeFile` calls, not the full read-modify-write cycle in `saveAttendanceRecord`.

## Goals / Non-Goals

**Goals:**
- Eliminate data loss when multiple users mark attendance simultaneously
- Keep the solution simple — no new dependencies, no database, no websockets
- Maintain the same user experience (tick a box, it saves)

**Non-Goals:**
- Real-time sync across browsers (other users' ticks appearing live)
- Replacing the JSON flat file storage
- Changing the "View Summary" or PDF export flows

## Decisions

### 1. Toggle endpoint instead of full-replace

**Decision**: New `POST /attendance/toggle` endpoint that accepts a single `memberId` + `checked` flag, instead of the full member list.

**Why over alternatives:**
- **Full-replace with locking** still breaks when browsers have stale state — User B's browser doesn't know about User A's ticks, so B's "full list" overwrites A's changes regardless of server-side locking
- **Toggle** eliminates the stale-state problem entirely because each request carries only its own intent ("add Alice" or "remove Alice"), not a snapshot of the world

### 2. Atomic lock around the full read-modify-write cycle

**Decision**: New `toggleAttendanceMember` function in `Database.fs` wraps the entire read → modify → write in a single `lock lockObj` block.

**Why**: The existing `readFile`/`writeFile` each lock independently, leaving a gap where another request can read stale data. One lock around the full cycle eliminates the file-level race.

### 3. Frontend debounce (300ms)

**Decision**: Add a 300ms debounce on the toggle call so rapid checkbox ticking batches into fewer requests.

**Why**: Without debounce, ticking 5 boxes in quick succession fires 5 separate POSTs. With debounce, the last tick in a rapid burst is the only one that fires. Since each toggle is independent (one member per request), this is safe — but if someone ticks multiple boxes within 300ms, we queue them and send sequentially after the debounce fires.

**Correction**: Actually, since each toggle is for a *different* member, we can't debounce them into one call. Instead, we'll collect all toggled checkboxes during the debounce window and send them as sequential requests (or a batch). Simplest approach: send each toggle immediately with no debounce, since the atomic lock handles concurrency. The server can handle the load.

**Revised decision**: No debounce. Send each toggle immediately. The atomic lock prevents races.

### 4. Keep `saveAttendanceRecord` for non-toggle paths

**Decision**: Keep the existing `saveAttendanceRecord` function for any remaining non-auto-save code paths (e.g., if View Summary still does a full save). Add atomic locking to it too, for safety.

## Risks / Trade-offs

- **More requests per session** — each checkbox = one POST, same as before. No change in volume. → Acceptable for a local/small-user app.
- **No cross-browser sync** — User A won't see User B's ticks appear in real time. They'll see them next time they load the page. → Acceptable trade-off; real-time sync would require websockets (out of scope).
- **Existing `saveAttendanceRecord` still has the race** — Mitigated by also adding atomic locking to it.
