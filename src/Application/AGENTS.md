# Application layer — agent notes

Use cases live here: mediator-pattern commands/queries, FluentValidation, DTOs, and domain-event handlers. Depends on `Domain` only (not `Infrastructure` or `Web`).

**Not MediatR:** dispatch is via the in-house implementation in `Common/Mediator/` (`IMediator`, `ISender`, `IRequest<T>`, pipeline behaviours). There is no MediatR NuGet package.

## Feature folder layout

Each bounded context is a top-level folder (e.g. `Recipes/`, `Cookbooks/`, `Users/`). Inside:

| Subfolder | Contents |
|-----------|----------|
| `Commands/<Name>/` | `*Command` record, `*CommandHandler`, optional `*CommandValidator` |
| `Queries/<Name>/` | `*Query` record, `*QueryHandler`; optional `*DbQuery.cs` for LINQ projections |
| `EventHandlers/` | `INotificationHandler<TDomainEvent>` (side effects, logging) |

**Naming:** command/query types end with `Command` / `Query`; validators end with `CommandValidator` / `QueryValidator`. Handlers are `sealed class`es implementing `IRequestHandler<,>`.

**Example paths:**

- `Recipes/Commands/CreateRecipe/CreateRecipe.cs`
- `Recipes/Commands/CreateRecipe/CreateRecipeCommandValidator.cs`
- `Recipes/Queries/GetRecipe/GetRecipe.cs`
- `Recipes/Queries/GetRecipe/GetDetailedDtoByIdDbQuery.cs`
- `Cookbooks/EventHandlers/CookbookCreatedEventHandler.cs`

## Adding a command or query

1. Create a folder under `Commands/` or `Queries/`.
2. Define a `sealed record` implementing `IRequest<TResponse>` (or `IRequest` for void).
3. Add a `sealed class` handler injecting `IApplicationDbContext` and other interfaces from `Common/Interfaces/`.
4. Add `*Validator` when input rules are non-trivial (registered automatically via `AddValidatorsFromAssembly`).
5. For new HTTP routes, also add an endpoint in `src/Web/Endpoints/` (see root `AGENTS.md`).

Handlers use `IApplicationDbContext` (not `ApplicationDbContext` directly). Throw `NotFoundException`, `ValidationException`, `ForbiddenAccessException`, etc. from `Common/Exceptions/`.

## Authorization

Pipeline: `AuthorizationBehaviour` runs on every request unless marked `[AllowAnonymous]`.

- **Default:** authenticated user required (`IUser.Id` must be set). No attribute needed on most commands/queries.
- **`[AllowAnonymous]`** on the request class — e.g. social login commands in `Users/Commands/`.
- **`[Authorize(Roles = "...")]`** or **`[Authorize(Policy = "...")]`** on the request class — custom attributes in `Common/Security/` (not ASP.NET’s). Policies are registered in `Infrastructure/DependencyInjection.cs` (`Policies.CanPurge`, etc.).

ASP.NET `[RequireAuthorization()]` on endpoints is separate; both layers can apply.

## DTOs and mappings

| Path | Purpose |
|------|---------|
| `Contracts/` | API DTOs (`RecipeDto`, `CreateRecipeDto`, `RecipeBriefDto`, …) |
| `Common/Mappings/` | Entity ↔ DTO mapping (`ToEntities()`, `ToDtos()`, `RecipeMapping.ToDetailedDto`) |
| `*/Queries/*DbQuery.cs` | `IQueryable` extension methods for DB projections |

**Contract / OpenAPI changes:** use the repo skill `.cursor/skills/update-contract/SKILL.md` — edit DTOs, update mappings, Debug-build `Web` to regenerate `specification.json`, then sync cookbook-mobile.

## Mediator pipeline

Registered via `AddMediator` in `DependencyInjection.cs`. See [mediator-and-pipeline.md](../../docs/architecture/mediator-and-pipeline.md) for execution order and domain-event dispatch.

Domain events: raise via `entity.AddDomainEvent(...)` in handlers; dispatch on `SaveChanges` (Infrastructure interceptors). React in `EventHandlers/` with `INotificationHandler<T>`.

## DbQuery pattern

Heavy LINQ stays out of handlers. Use an `internal static` class with C# 14 `extension(IQueryable<T>)` methods, e.g. `GetDetailedDtoByIdDbQuery.cs`. Handlers call `context.Recipes.GetDetailedDtoById(...)`.

## Shared helpers

- `Common/Extensions/` — query filters (`CookbookQueryExtensions`, etc.)
- `Common/ImageUtilities.cs` — image URL handling
- `GlobalUsings.cs` — global usings for the project

## Tests

| Change type | Test project |
|-------------|----------------|
| Handler / validator / mapping logic | `tests/Application.UnitTests/` |
| End-to-end use case | `tests/Application.FunctionalTests/` — see `tests/AGENTS.md` |

Follow `.cursor/rules/unit-test-patterns.mdc` for test style (single assert, scenario classes).
