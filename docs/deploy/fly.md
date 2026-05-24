# Fly.io deployment

The API runs on [Fly.io](https://fly.io) as a Docker app, backed by a separate Fly Postgres app.

- **App**: `sharedcookbook-api` -> `https://sharedcookbook-api.fly.dev`
- **DB app**: `sharedcookbook-db` (Fly Postgres, unmanaged, single-node "Development" config with scale-to-zero)
- **Region**: `iad` (Ashburn, VA). Both apps live in the same region so they talk over Fly's private network.

## `fly.toml`

The full file is at the repo root: [fly.toml](../../fly.toml). Key sections:

### Build

Fly uses the repo-root [Dockerfile](../../Dockerfile) by default (no `[build]` section in `fly.toml`). It is a 3-stage build: SDK build -> publish -> ASP.NET runtime with [recipe from photo](../features/recipe-from-photo.md) baked in.

### Env

```toml
[env]
  ASPNETCORE_ENVIRONMENT = "Production"
  ASPNETCORE_URLS = "http://+:8080"
```

Public, non-secret config. `ASPNETCORE_URLS` makes Kestrel listen on port 8080 inside the container. Anything sensitive (DB password, API keys) goes in **secrets**, not here.

### HTTP service + health check

```toml
[http_service]
  internal_port = 8080
  force_https = true
  auto_stop_machines = "stop"
  auto_start_machines = true
  min_machines_running = 0

  [[http_service.checks]]
    interval = "15s"
    timeout = "5s"
    grace_period = "30s"
    method = "GET"
    path = "/health"
```

- `force_https`: Fly's edge proxy terminates TLS and redirects HTTP -> HTTPS. The app itself only sees plain HTTP from the proxy - that's why [Program.cs](../../src/Web/Program.cs) does **not** call `UseHttpsRedirection()` in Production (would cause a redirect loop).
- `auto_stop_machines = "stop"` + `min_machines_running = 0`: the API machine sleeps when idle to save money. First request after idle has a ~1-2s cold start.
- Health check hits `/health`, which is registered in [Program.cs](../../src/Web/Program.cs) via `app.UseHealthChecks("/health")`.

### VM size

```toml
[[vm]]
  cpus = 1
  memory = "512mb"
```

Smallest practical size for a .NET API. Scale up later with `fly scale memory 1024`.

## Postgres

Created with **Fly Postgres (unmanaged)**.

```powershell
fly postgres create --name sharedcookbook-db --region iad
# Development (single node), 1 GB volume, scale-to-zero after 1 hour
```

### Version

| | |
| --- | --- |
| **Postgres** | **17.7** |
| **Fly machine image** | `flyio/postgres-flex:17.2` |
| **Running server** (check live) | `fly ssh console -a sharedcookbook-db -C "postgres --version"` |

Functional tests use the same major/patch via Testcontainers: `postgres:17.7` in [PostgreSQLTestcontainersTestDatabase.cs](../../tests/Application.FunctionalTests/PostgreSQLTestcontainersTestDatabase.cs

### Connection string

`fly postgres attach` sets `DATABASE_URL` on the API in `postgres://user:pass@host.flycast:5432/db?sslmode=disable` format. ASP.NET reads `ConnectionStrings:DefaultConnection` instead (see [src/Infrastructure/DependencyInjection.cs](../../src/Infrastructure/DependencyInjection.cs)), so we set it explicitly in Npgsql format:

```powershell
fly secrets set "ConnectionStrings__DefaultConnection=Host=sharedcookbook-db.flycast;Port=5432;Database=sharedcookbook_api;Username=sharedcookbook_api;Password=REDACTED;Ssl Mode=Disable" --app sharedcookbook-api
```

`sharedcookbook-db.flycast` only resolves **inside** Fly's private network. From your machine, use `fly proxy 5432 -a sharedcookbook-db` (then connect to `localhost:5432`) or `fly postgres connect -a sharedcookbook-db` for an interactive `psql`.

### Migrations

EF Core migrations run automatically on startup in [src/Infrastructure/Data/ApplicationDbContextInitialiser.cs](../../src/Infrastructure/Data/ApplicationDbContextInitialiser.cs):

```csharp
await _context.Database.MigrateAsync();
```

Deploys do **not** reset data. The Postgres app and its volume are untouched when the API redeploys.

### Cold start (scale-to-zero)

The DB app is configured to scale to zero after idle time. On deploy or first traffic, the API can start before Postgres is listening. Symptoms in `fly logs`:

- `NpgsqlException: Exception while reading from stream` with inner `EndOfStreamException` during `MigrateAsync`
- `ApplicationDbContext` health check unhealthy, then passing after the DB machine wakes

Startup migrations retry transient connection errors (see `ApplicationDbContextInitialiser`).

## Secrets

ASP.NET maps `Section__SubKey` env vars to nested config (`Section:SubKey`). All sensitive config is set this way:

```powershell
fly secrets set `
  "ConnectionStrings__DefaultConnection=..." `
  "Authentication__Google__ClientId=..." `
  "Authentication__Apple__BundleId=com.cookbookmobile" `
  "ImageUploadOptions__BucketName=..." `
  "ImageUploadOptions__AwsAccessKeyId=..." `
  "ImageUploadOptions__AwsSecretAccessKey=..." `
  "ImageUploadOptions__Region=us-east-1" `
  "EmailApiOptions__ApiKey=..." `
  "EmailApiOptions__BaseUrl=..." `
  "EmailApiOptions__Domain=..." `
  "EmailApiOptions__From=..." `
  "RecipeUrlParserOptions__ApiKey=..." `
  "RecipeUrlParserOptions__BaseUrl=..." `
  "AiRecipeParserOptions__ApiKey=..." `
  "AiRecipeParserOptions__Model=..." `
  --app sharedcookbook-api
```

### Gotchas

- **Dots are not allowed in Fly secret names.**
- Setting a secret triggers a rolling restart of the app. To stage many changes, set them in one `fly secrets set` call.
- List current secrets (names only, values are never displayed):
  ```powershell
  fly secrets list --app sharedcookbook-api
  ```
- Remove one:
  ```powershell
  fly secrets unset SOME__KEY --app sharedcookbook-api
  ```

## Manual deploy (escape hatch)

CI/GitHub auto-deploy is the normal path (see [github-actions.md](./github-actions.md)). When you need to deploy without going through GitHub:

```powershell
cd C:\Users\brett\git\SharedCookbook
fly deploy --app sharedcookbook-api
```

`fly deploy` reads `fly.toml`, builds the image (on Fly's remote builders by default, no local Docker needed), pushes it, and rolls out new machines while running the `/health` check.

## Operating the app


| Goal                    | Command                                          |
| ----------------------- | ------------------------------------------------ |
| Live logs               | `fly logs --app sharedcookbook-api`              |
| Snapshot of recent logs | `fly logs --app sharedcookbook-api --no-tail`    |
| App + machine status    | `fly status --app sharedcookbook-api`            |
| Restart                 | `fly apps restart sharedcookbook-api`            |
| Shell into a machine    | `fly ssh console --app sharedcookbook-api`       |
| Releases / history      | `fly releases --app sharedcookbook-api`          |
| Roll back               | `fly deploy --image <previous-image-tag>`        |
| Scale memory            | `fly scale memory 1024 --app sharedcookbook-api` |
| Scale count             | `fly scale count 2 --app sharedcookbook-api`     |
| Open psql               | `fly postgres connect -a sharedcookbook-db`      |
| DB tunnel for GUI tools | `fly proxy 5432 -a sharedcookbook-db`            |


## How redeploys treat the database


| Action                               | Effect on DB data                                                    |
| ------------------------------------ | -------------------------------------------------------------------- |
| `git push` to `master` -> auto-deploy | Unchanged                                                            |
| `fly deploy` (API)                   | Unchanged                                                            |
| EF migration on startup              | Schema updated, rows kept (unless a migration explicitly drops them) |
| `fly apps destroy sharedcookbook-db` | **Wiped**                                                            |
| `fly volumes destroy <id>`           | **Wiped**                                                            |


Reference: [Fly.io configuration docs](https://fly.io/docs/reference/configuration/).

