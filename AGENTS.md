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
| Local DB | `compose.yaml` | Postgres only, or use with `web` service for full container run |
| Solution | `SharedCookbook.slnx` | XML solution (not `.sln`); root also has `Directory.Build.props`, `Directory.Packages.props` |
| SDK pin | `global.json` | .NET 10.0.100, `rollForward: latestFeature` |

## Prerequisites

- .NET SDK matching `global.json` (10.0.100+ as configured).
- PostgreSQL for local dev: `appsettings.Development.json` uses `127.0.0.1:5432`, database `SharedCookbookDb`, user `postgres`, password `admin` — aligned with `compose.yaml` `database` service.

## Run locally

1. Start Postgres (from repo root):

   ```bash
   docker compose up database
   ```

   Or run only the DB in the background as you prefer.

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

## Docker (full stack)

From repo root, `docker compose up` builds `web` (depends on `database`). Compose maps **HTTP** on host port **8080** to the container; inside compose, the app uses the `database` hostname for Postgres (not `localhost`).

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
