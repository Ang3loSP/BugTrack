# BugTrack

A bug & test case management system built with ASP.NET Core MVC, EF Core & SQL Server — modelling the core workflow QA teams use to log defects, design test cases, track execution results & link failures back to bugs.

Built as a portfolio project demonstrating practical understanding of software testing & quality assurance workflows, alongside coursework on the Full Stack Web & Software Development Occupational Certificate (NQF Level 5) at the Academic Institute of Excellence.

## Features

- **Bug tracking** — create, edit, delete & view bugs with Title, Description, Steps to Reproduce, Severity, Priority, Assigned To & a creation/update timestamp.
- **Bug status lifecycle** — enforced state machine (New → In Progress → Fixed → Retest → Closed / Reopened) via `BugStatusService`. Illegal transitions (e.g. jumping straight from New to Closed via Retest) are blocked server-side.
- **Test case management** — create, edit, delete & view test cases with Module, Preconditions, Steps, Expected Result & Priority.
- **Test run history** — every execution of a test case is logged as its own record (date, tester, actual result, Pass/Fail/Blocked, notes), rather than a single overwritten status field.
- **Bug-from-failed-run linking** — logging a Fail result against a test case offers the option to generate a linked bug directly, with the relationship tracked in both directions.
- **Dashboard** — summary view of total & open bugs by priority/status, total test cases by priority, total test runs & an overall pass rate.
- **Filtering & search** — bug list filterable by status, priority & severity; test case list filterable by module & priority, both with keyword search.

## Tech Stack

- ASP.NET Core 8 MVC, C#
- Entity Framework Core 8 (code-first, migrations)
- SQL Server (LocalDB for development)
- Razor Views, Bootstrap 5
- jQuery / jQuery Validation (client-side form validation)

## Data Model

| Entity | Key Fields |
|---|---|
| `Bug` | Id, Title, Description, StepsToReproduce, Severity, Priority, Status, CreatedDate, UpdatedDate, AssignedTo, LinkedTestRunId |
| `TestCase` | Id, Title, Module, Preconditions, Steps, ExpectedResult, Priority, CreatedDate, UpdatedDate |
| `TestRun` | Id, TestCaseId, ExecutedDate, ExecutedBy, ActualResult, Result, Notes, LinkedBugId |

A `TestCase` has many `TestRun`s. A `TestRun` may optionally link to a `Bug` if its result was a Fail. A `Bug` may optionally reference the `TestRun` that originated it.

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server LocalDB (included with Visual Studio) or a SQL Server instance

### Setup
1. Clone the repository.
2. Update the `DefaultConnection` string in `appsettings.json` if you're not using LocalDB.
3. Apply migrations & seed the database (this also happens automatically on first run via `Program.cs`):
   ```
   dotnet ef database update
   ```
4. Run the project:
   ```
   dotnet run
   ```
5. Navigate to `https://localhost:[port]` to view the dashboard.

On first run, the database is seeded with a small set of sample test cases, bugs & test runs so the dashboard & relationships are populated immediately.

## Project Status

This is a v1 build covering the core & extended scope defined in the project brief (status lifecycle, severity/priority split, test run history, bug-test run linking, dashboard & filtering). Out of scope for this version: multi-user accounts & role-based permissions, automated test execution, a Kanban-style board UI & notification alerts. These are documented as potential v2 additions in the project brief, not implemented here.

No automated unit tests are included in this version.

## Author

Angelo Puza
[github.com/Ang3loSP](https://github.com/Ang3loSP) | [angelo-puza-portfolio.netlify.app](https://angelo-puza-portfolio.netlify.app)
