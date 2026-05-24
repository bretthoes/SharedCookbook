# ApplicationUser in Infrastructure

`ApplicationUser` ([src/Infrastructure/Identity/ApplicationUser.cs](../../src/Infrastructure/Identity/ApplicationUser.cs)) is the persisted user record for this API. It behaves like a domain entity in many ways (stored in Postgres, referenced by cookbook data, has app-specific fields such as `DisplayName`), but it is defined in the **Infrastructure** layer because it **inherits ASP.NET Core Identity’s `IdentityUser`**.

This document explains that placement, the boundaries we draw around it, and the tradeoffs.

## What it is

```csharp
public sealed class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
```

Identity supplies the core account model: `Id`, `Email`, `UserName`, password hash, lockout, external logins, roles, and the `AspNetUsers` / `AspNetRoles` / … table set. We extend it only where the product needs extra columns.

The type is registered with Identity and EF in [DependencyInjection.cs](../../src/Infrastructure/DependencyInjection.cs) (`AddIdentityCore<ApplicationUser>()`, `AddEntityFrameworkStores<ApplicationDbContext>()`). [ApplicationDbContext](../../src/Infrastructure/Data/ApplicationDbContext.cs) subclasses `IdentityDbContext<ApplicationUser>` so **one** context and **one** database hold both Identity tables and cookbook domain tables.

## Why not Domain?

Clean Architecture and DDD usually put entities in `Domain` with **no** dependencies on frameworks. `ApplicationUser` cannot follow that rule without giving up or reimplementing Identity:


| Constraint                                                  | Effect                                                               |
| ----------------------------------------------------------- | -------------------------------------------------------------------- |
| `IdentityUser` lives in `Microsoft.AspNetCore.Identity`     | Domain would reference ASP.NET packages                              |
| `UserManager<TUser>`, stores, bearer tokens, external login | Expect `TUser : IdentityUser` (or a compatible substitute)           |
| EF Core Identity integration                                | `IdentityDbContext<TUser>` and migrations assume the Identity schema |


Moving `ApplicationUser` into Domain would either **pollute Domain with infrastructure concerns** or force a **duplicate user model** (domain `User` + infrastructure `ApplicationUser` + mapping on every read/write). This project follows the common Jason Taylor / Clean Architecture template approach: **the Identity-backed user type is an infrastructure concern**.

Cookbook concepts (`Cookbook`, `Recipe`, `CookbookMembership`, …) remain in `Domain` with integer keys and no Identity references.

## How other layers refer to “the user”

Application and Domain code must not take a dependency on `ApplicationUser`. Boundaries:


| Layer              | User concept                | Implementation                                                                                                                                                                       |
| ------------------ | --------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Web**            | Current HTTP principal      | [CurrentUser](../../src/Web/Services/CurrentUser.cs) implements `IUser` (`Id`, `Roles`)                                                                                              |
| **Application**    | Use cases                   | `IUser` for “who is calling”; `IIdentityService` for lookups and account mutations (email, display name, create/delete)                                                              |
| **Application**    | Read models that join users | `IIdentityRepository` — see below                                                                                                                                                    |
| **Infrastructure** | Persistence + Identity APIs | `ApplicationUser`, `UserManager<ApplicationUser>`, `IdentityService`, `ExternalLoginService`                                                                                         |
| **Domain**         | Ownership and audit         | **String user ids** on auditable entities (`CreatedBy`, `LastModifiedBy`) and invitation fields (`RecipientPersonId`); no navigation properties to `ApplicationUser` on domain types |


Domain entities do not declare `ApplicationUser` navigation properties. Foreign keys to `AspNetUsers` are configured only in Infrastructure, e.g. [CookbookInvitationConfiguration](../../src/Infrastructure/Data/Configurations/CookbookInvitationConfiguration.cs) uses `HasOne<Identity.ApplicationUser>()` without polluting the `CookbookInvitation` class.

### IdentityRepository

Some queries need joins to `AspNetUsers` (memberships and invitations with display names). Those live in [IdentityRepository](../../src/Infrastructure/Identity/IdentityRepository.cs), which is intentionally Infrastructure-only. The class comment states the tradeoff: **Application stays free of Identity types**, at the cost of **extra repository surface** and **join logic in Infrastructure projections** ([InvitationProjections](../../src/Infrastructure/Identity/Projections/InvitationProjections.cs), etc.).

Treat `IdentityRepository` as **read-oriented**; writes to user accounts go through `IIdentityService` / Identity’s `UserManager`, not arbitrary EF updates from feature code.

## Tradeoffs

### Chosen approach (Identity user type in Infrastructure)

**Pros**

- Full use of Identity: password rules, email confirmation, bearer tokens, external login, roles, migrations aligned with the ecosystem.
- Single `ApplicationDbContext` and transactional consistency between cookbook data and user rows.
- Domain and Application projects stay testable without pulling in `Microsoft.AspNetCore.Identity`.
- Matches the rest of the solution template and Microsoft’s documented patterns.

**Cons**

- **Split “entity” story**: cookbook aggregates are domain entities; the user aggregate is effectively owned by Identity, not `Domain/Entities`.
- **String foreign keys** to `AspNetUsers` without rich domain modeling of `Person` / `User`.
- **Join queries** concentrated in `IdentityRepository` rather than on domain navigation properties.
- **Schema coupling**: Identity’s table layout and migration behavior are part of our database contract; upgrading Identity packages can affect migrations.
- `**ApplicationUser` is not where domain invariants live** — business rules about cookbooks/memberships stay on domain types; profile rules stay in Application handlers + `IIdentityService`.

## Practical guidelines

1. **Add app-specific user fields on `ApplicationUser`** when they are account/profile data stored with Identity (e.g. `DisplayName`). Run an EF migration from `src/Infrastructure` per [add-migration skill](../../.cursor/skills/add-migration/SKILL.md).
2. **Do not reference `ApplicationUser` from Application or Domain.** Use `IUser`, `IIdentityService`, or DTOs.
3. **Reference users from domain code by `string` id** (typically `IUser.Id` or ids returned from `IIdentityService`), same as audit columns on `BaseAuditableEntity`.
4. **Configure relationships to `ApplicationUser` in `Infrastructure/Data/Configurations`**, not on domain entity classes.
5. **Prefer `IIdentityService` for account changes**; avoid updating `context.People` in feature handlers unless there is a strong, documented reason.
6. For login and token flows, see [social-sign-in.md](../features/social-sign-in.md); those endpoints still resolve to the same `ApplicationUser` rows.

## Related code map


| Path                                                    | Role                                                    |
| ------------------------------------------------------- | ------------------------------------------------------- |
| `src/Infrastructure/Identity/ApplicationUser.cs`        | User entity type                                        |
| `src/Infrastructure/Data/ApplicationDbContext.cs`       | `DbSet<ApplicationUser> People`, Identity + domain sets |
| `src/Infrastructure/Identity/IdentityService.cs`        | `IIdentityService`                                      |
| `src/Infrastructure/Identity/IdentityRepository.cs`     | Join queries for listings                               |
| `src/Application/Common/Interfaces/IUser.cs`            | Current caller                                          |
| `src/Application/Common/Interfaces/IIdentityService.cs` | Account operations                                      |
| `src/Domain/Common/BaseAuditableEntity.cs`              | `CreatedBy` / `LastModifiedBy` as strings               |


