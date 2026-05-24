# Infrastructure layer — agent notes

Implements Application interfaces: EF Core, ASP.NET Identity, file storage, email, OCR, AI, URL parsing. Depends on `Application` and `Domain`.

## Subsystem map

| Folder / area | Implements | Notes |
|---------------|------------|-------|
| `Data/` | `ApplicationDbContext`, EF configs, interceptors, migrations | Postgres via Npgsql |
| `Data/Configurations/` | `IEntityTypeConfiguration<T>` per entity | No data annotations on domain entities |
| `Data/Interceptors/` | `AuditableEntityInterceptor`, `DispatchDomainEventsInterceptor` | Run on `SaveChanges` |
| `Data/Migrations/` | EF migrations | **Zero-padded numeric prefix** — see skill below |
| `Identity/` | Users, roles, bearer tokens, Google/Apple/Facebook login | `ApplicationUser`, `ExternalLoginService` — see [application-user.md](../../docs/architecture/application-user.md) |
| `FileStorage/` | `S3ImageUploader` | `ImageUploadOptions` in config |
| `Email/` | `EmailSender` | External email API |
| `Ocr/` | `TesseractOcrService` | Native tessdata under `src/Web/wwwroot/tessdata/` |
| `Ai/` | `OpenAiRecipeParser` | Voice transcript → recipe draft |
| `RecipeUrlParser/` | `SpoonacularApiParser` | URL → recipe draft |
| `Security/` | `Sha256TokenFactory` | Invitation/share-link tokens |

Registration is in `DependencyInjection.cs` (`AddInfrastructureServices`).

## Database and migrations

| Path | Purpose |
|------|---------|
| `Data/ApplicationDbContext.cs` | `DbSet<T>` per entity (expression-bodied) |
| `Data/ApplicationDbContextInitialiser.cs` | Seed data, dev bootstrap |
| `Domain/Entities/` | Entity definitions — edit here first |

**Adding or changing schema:** use `.cursor/skills/add-migration/SKILL.md` (entity → DbContext → `dotnet ef` → rename with `000000000000NN_` prefix → update `[Migration]` attribute). Run EF commands from `src/Infrastructure/` with `--startup-project ../Web`.

Migrations run on API startup in dev/deploy (`Program.cs` → `InitialiseDatabaseAsync`).

## Feature deep dives

Non-obvious flows (external APIs, native deps, deploy caveats) are documented for humans in `docs/features/`:

| Doc | Topic |
|-----|-------|
| [recipe-from-photo.md](../../docs/features/recipe-from-photo.md) | Tesseract OCR |
| [recipe-from-url.md](../../docs/features/recipe-from-url.md) | Spoonacular + optional S3 re-host |
| [recipe-from-voice.md](../../docs/features/recipe-from-voice.md) | OpenAI parsing |
| [social-sign-in.md](../../docs/features/social-sign-in.md) | Google / Apple / Facebook |
| [cookbook-share-link.md](../../docs/features/cookbook-share-link.md) | Hashed invitation tokens |

Deploy and Fly.io: [docs/deploy/](../../docs/deploy/).

## Configuration

Secrets and options bind from `src/Web/appsettings*.json` (connection string `DefaultConnection`, `ImageUploadOptions`, `RecipeUrlParserOptions`, `AiRecipeParserOptions`, auth sections, etc.). Never commit secrets — use `fly secrets set` in production (see deploy docs).

## Tests

| Layer | Project |
|-------|---------|
| Parsers, token factory, small services | `tests/Infrastructure.UnitTests/` |
| DI / DB integration (light) | `tests/Infrastructure.IntegrationTests/` |
| Full stack | `tests/Application.FunctionalTests/` — see `tests/AGENTS.md` |

## Application contract changes

DTO and OpenAPI work lives in `src/Application/Contracts/` — use `.cursor/skills/update-contract/SKILL.md`, not this layer, unless the change also needs new infra services or config.
