# Changelog

All notable changes to this project are documented in this file.

## [Unreleased]

## [1.1.0] - 2026-09-14

### Added
- "Midnight Indigo Liquid Glass" UI theme — a full re-skin built on a CSS-variable design system:
  - Glass panels, film-grain atmosphere and pointer-tracked specular highlights over a dark indigo canvas.
  - Custom typography: Instrument Serif (display), Space Grotesk (UI), IBM Plex Mono (IDs, dates, labels).
  - Re-styled Bootstrap primitives (buttons, badges, tables, alerts, forms, cards) plus motion with `prefers-reduced-motion` support.
- Floating glass navigation dock with active-section state.
- Rebuilt dashboard: stat tiles, animated priority bars, status chips, a QA-ticket "receipt" panel for recent test runs, and a recent-bugs list.

### Changed
- Rewrote all Bugs, TestCases, TestRuns and Error views on the new design primitives.
- Replaced the scaffolded TestCases Create form with the same safe-form pattern used elsewhere (drops the previously exposed `CreatedDate`, `UpdatedDate` and `TestRuns` fields).

## [1.0.1] - 2026-09-13

### Fixed
- TestRuns Create: the "Create bug from failed test" panel now appears only when the result is **Fail** (was comparing against the wrong enum value, so the panel never showed).
- Bugs and TestCases delete: prevented deletion when records have linked test runs / execution history, with a TempData error message and a `DbUpdateException` guard. Delete views warn and disable the submit button in that case.
- Bugs Edit: no longer accepts overposted `Status`, `CreatedDate` or `LinkedTestRunId` — only the editable fields bind, and server-side values are preserved from the existing record (closed an HTTP status-bypass).
- TestCases Edit: now binds only the editable fields and preserves `CreatedDate` (overposted values previously replaced server data).
- Removed a stale `TestRunId` foreign key/index/column on `Bugs` that duplicated `LinkedTestRunId`, and the orphaned `BugsFromThisRun` navigation on `TestRun` (migration `RemoveStaleBugTestRunLink`).
- TestRuns Create: save is now wrapped in an explicit transaction with rollback on failure (no partially-linked bug on a failed submit).
- Dashboard: rewritten with grouped aggregate queries; pass rate is now computed as `Pass / (Pass + Fail)` instead of counting all runs.
- Bugs Details: fixed a latent 500 error when no status transitions are available (`AvailableTransitions.Count == 0`).
- jQuery is now loaded in the layout, restoring client-side validation and the Fail-panel toggle that previously never executed.

## [1.0.0] - 2026-06-24

### Added
- Initial release: bug & test case management with the status lifecycle, test run history, bug-from-failed-run linking, dashboard, and filtering/search.