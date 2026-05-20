# Tests — agent notes

NUnit across six projects. Run from repo root: `dotnet test`.

## Which project to use

| Project | Use when testing… |
|---------|-------------------|
| `Application.UnitTests` | Handlers, validators, behaviours, `Common/Mappings`, extensions |
| `Application.FunctionalTests` | Full use cases through `ISender` + real Postgres (Testcontainers) |
| `Domain.UnitTests` | Entities, value objects, domain invariants |
| `Infrastructure.UnitTests` | Parsers, token factory, invitation responder, other infra helpers |
| `Infrastructure.IntegrationTests` | Infrastructure wiring with mocks (lighter than functional) |
| `Tests.Shared` | Shared constants and builders (`TestData`, `RecipeTestData`) — not a test assembly itself |

Mirror Application feature names in functional tests (e.g. `Recipes/Commands/CreateRecipeTests/`).

## Style conventions

Follow `.cursor/rules/unit-test-patterns.mdc`:

- One assert per `[Test]` method
- Scenario-based classes (`WhenRecipeIsValid`, `WhenCookbookIsInvalid`)
- Shared setup via `[OneTimeSetUp]` when immutable; `[SetUp]` per test when state must reset
- Constants for expected values

## Functional tests (`Application.FunctionalTests`)

### Prerequisites

- **Docker** must be running — `TestDatabaseFactory` uses **Testcontainers** (`PostgreSQLTestcontainersTestDatabase`).
- To use a local Postgres instead, switch `TestDatabaseFactory` to `PostgreSQLTestDatabase` and configure `appsettings.json` (comment in `TestDatabaseFactory.cs`).

### Harness

| Type | Role |
|------|------|
| `Testing` (`[SetUpFixture]`) | One-time DB + `CustomWebApplicationFactory`; static helpers |
| `BaseTestFixture` | `[SetUp]` calls `ResetState()` before each test; extend for functional tests |
| `CustomWebApplicationFactory` | `WebApplicationFactory<Program>` with test Postgres + mocked `IUser` |
| `Tests.Shared` | `TestData.AnyNonEmptyString`, `RecipeTestData.GetSimpleCreateRecipeDto`, etc. |

Import helpers: `using static Testing;`

### Common helpers (`Testing.cs`)

```csharp
await RunAsDefaultUserAsync();           // creates test@local user, sets IUser.Id
await RunAsUserAsync(user, pass, roles);

await SendAsync(commandOrQuery);         // dispatches through in-house IMediator (Common/Mediator/)
await CreateSimpleRecipe();              // cookbook + recipe shortcut

await FindAsync<TEntity>(keyValues);
await AnyAsync<TEntity>(predicate);
await SingleAsync<TEntity>(predicate);
await ListAsync<TEntity>();
await AddAsync(entity);

await ResetState();                      // Respawn DB + clear user (also in BaseTestFixture.SetUp)
```

Functional test classes should inherit `BaseTestFixture` and be marked `[NonParallelizable]` (inherited from base) to avoid DB races.

### Typical flow

```csharp
public class WhenRecipeIsValid : BaseTestFixture
{
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await ResetState();
        await RunAsDefaultUserAsync();
        var id = await CreateSimpleRecipe();
        _actual = await FindAsync<Recipe>([id]);
    }

    [Test]
    public void ShouldHaveTitle() => Assert.That(_actual!.Title, Is.EqualTo(ExpectedTitle));
}
```

Use `SendAsync` to exercise handlers; use `FindAsync` / `AnyAsync` to assert persisted state.

## Unit tests

- **Application:** mock `IApplicationDbContext`, `IUser`, etc. with Moq; test handler/validator in isolation.
- **Domain:** no mocks; pure entity/value-object tests under `Entities/`, `ValueObjects/`.
- **Infrastructure:** test parsers and services with fixture data under `Infrastructure.UnitTests/` (e.g. `RecipeUrlParserTests/`).

## After Application changes

1. Add or update unit tests for new validation/mapping logic.
2. Add functional tests for new commands/queries that touch persistence or auth.
3. Run `dotnet test` before opening a PR.
