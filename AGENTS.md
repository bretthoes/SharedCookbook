# SharedCookbook — agent notes

Backend API for the [cookbook-mobile](https://github.com/bretthoes/cookbook-mobile) app. OpenAPI 3.1; Clean Architecture / DDD (Jason Taylor template lineage).

## Repo map

| Area | Path | Notes |
|------|------|--------|
| Web host | `src/Web/` | ASP.NET Core entry, `appsettings*.json`, `Properties/launchSettings.json` |
| OpenAPI spec | `src/Web/wwwroot/api/specification.json` | Source for mobile TypeScript codegen (sibling repo) |
| Application | `src/Application/` | Use cases, validators |
| Domain | `src/Domain/` | Entities, domain logic |
| Infrastructure | `src/Infrastructure/` | EF Core, external services; migrations under `Data/Migrations/` |
| Tests | `tests/*` | Unit, integration, functional (NUnit) |
| Solution | `SharedCookbook.slnx` | XML solution (not `.sln`); root also has `Directory.Build.props`, `Directory.Packages.props` |
| Production image | `Dockerfile` | Fly.io deploy only; see `docs/deploy/` |
| SDK pin | `global.json` | .NET 10.0.100, `rollForward: latestFeature` |

## Prerequisites

- .NET SDK matching `global.json` (10.0.100+ as configured).
- PostgreSQL for local dev: `src/Web/appsettings.Development.json` expects Postgres at `127.0.0.1:5432`, database `SharedCookbookDb`, user `postgres`, password `admin`. Ensure a matching instance is running before starting the API.

## Run locally

1. Ensure Postgres is running and matches the connection string in `appsettings.Development.json`.

2. Run the API:

   ```bash
   cd src/Web
   dotnet watch run
   ```

3. URLs (see `Properties/launchSettings.json`):

   - HTTPS: `https://localhost:5001`
   - HTTP: `http://localhost:5000`
   - Health: `GET https://localhost:5001/health` (or HTTP on 5000)

The mobile dev client is configured for **HTTP on port 5000** against the host (`http://127.0.0.1:5000/api` on iOS simulator; `http://10.0.2.2:5000/api` on Android emulator). Keep that profile when testing with cookbook-mobile.

## Build and test

From repo root:

```bash
dotnet build -tl
dotnet test
```

## Mobile / OpenAPI

With a typical sibling layout (`.../git/SharedCookbook` and `.../git/cookbook-mobile`), the mobile repo runs `pnpm run generate:api`, which reads:

`../SharedCookbook/src/Web/wwwroot/api/specification.json`

After API or contract changes, regenerate types in cookbook-mobile and adjust wrappers there if needed.
