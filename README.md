# Flight Status Tracker

A local, offline-capable Flight Status lookup application for the SkyRoute platform.

## Stack

- Backend: .NET 8 Minimal API / C#
- Frontend: plain HTML/CSS/JavaScript
- Tests: xUnit
- No external flight APIs, credentials, authentication, or persistence

## Repository structure

```text
flight-status/
├── README.md
├── spec.md
├── prompts.md
├── reflection.md
├── FlightStatus.sln
├── FlightStatus.Api/
│   ├── FlightStatus.Api.csproj
│   ├── Program.cs
│   ├── Models/
│   │   └── FlightStatusModels.cs
│   ├── Providers/
│   │   ├── IFlightStatusProvider.cs
│   │   ├── AeroTrackProvider.cs
│   │   └── QuickFlightProvider.cs
│   └── Services/
│       └── FlightStatusService.cs
├── FlightStatus.Tests/
│   ├── FlightStatus.Tests.csproj
│   └── FlightStatusServiceTests.cs
└── flight-status-ui/
    ├── index.html
    ├── app.js
    └── styles.css
```

## Prerequisites

- .NET 8 SDK or later
- A modern browser

No Node.js, Angular CLI, database, API key, or external service is required.

## Run the API

From the repository root:

```bash
dotnet restore
dotnet run --project FlightStatus.Api
```

The API starts on the URLs printed by ASP.NET Core, normally:

```text
http://localhost:5000
https://localhost:5001
```

If a different port is selected, update `API_BASE_URL` in `flight-status-ui/app.js`.

## Run the UI

The simplest option is to serve the UI folder with any local static server. For example, if Python is installed:

```bash
cd flight-status-ui
python -m http.server 5500
```

Open:

```text
http://localhost:5500
```

The UI calls the local API only. No external network dependency is used.

## API

```http
GET /flights/status?flightNumber={code}&date={yyyy-MM-dd}
```

Example:

```text
GET http://localhost:5000/flights/status?flightNumber=AR101&date=2026-08-19
```

The API returns HTTP 400 when `flightNumber` or `date` is missing/invalid.

## Deterministic demo cases

The two stub providers intentionally return deterministic data so the application can be demonstrated repeatedly.

Useful flight numbers:

| Flight | Scenario |
|---|---|
| `AR101` | Both providers respond; AeroTrack is newer and contains full details |
| `QF202` | QuickFlight is the only usable provider |
| `AR303` | AeroTrack reports a delayed flight |
| `QF404` | QuickFlight reports a cancelled flight |
| `AR505` | AeroTrack reports a diverted flight |
| `QF606` | Both providers respond; QuickFlight has the later update |
| `ZZ999` | Neither provider returns a result; API returns `Unknown` |

The date can be any valid `yyyy-MM-dd` date.

## Tests

```bash
dotnet test
```

The tests focus on the core merge and business rules:

- newer `lastUpdatedUtc` wins
- only one provider responds
- neither provider responds
- 400 validation
- deterministic status scenarios

## Assumptions

1. Provider status vocabularies are represented by the stub implementations because no real provider contracts are supplied.
2. The provider layer owns provider-specific vocabulary and normalization.
3. `lastUpdatedUtc` is used as the source freshness indicator when merging.
4. `Unknown` is returned when no provider has a usable status.
5. Date is validated as an ISO `yyyy-MM-dd` value.
6. The UI displays AeroTrack-only fields only when they exist in the unified response.
7. Since there is no persistence requirement, all demo data is held in memory.

## Clean clone

A clean clone only needs the .NET 8 SDK and a browser. Run the API, start the static UI server, and open the UI. No secrets are required.

