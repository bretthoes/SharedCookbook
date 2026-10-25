# Domain — agent notes

Entities, value objects, enums, and domain events. This project references nothing in `Application`, `Infrastructure`, or `Web`.

Persistence details stay in `Infrastructure/Data/Configurations/`. A new column is an entity change plus an EF configuration and a migration (`.cursor/skills/add-migration/SKILL.md`).

## Layout

| Folder | Contents |
| --- | --- |
| `Entities/` | Aggregates and children: `Cookbook`, `Recipe`, `CookbookMembership`, invitations, notifications |
| `ValueObjects/` | `Timing`, `DietaryTags`, `MealTypes`, `TokenDigest`, `Colour` |
| `Enums/` | `MembershipTier`, `InvitationStatus`, `CookbookNotificationActionType` |
| `Events/` | `BaseEvent` subclasses raised with `AddDomainEvent` |
| `Exceptions/` | Domain rule failures (`InvitationException`, hash/salt length) |
| `Constants/` | ASP.NET Identity `Roles` and `Policies` — not cookbook membership |
| `Common/` | `BaseEntity`, `BaseAuditableEntity`, `BaseInvitation`, `ValueObject` |

## Entities

`BaseEntity` has a `Guid Id` and a domain-event list. `[NotMapped]` on `DomainEvents` is the one data annotation that belongs here. Table names, column names, and max lengths are EF configuration, and they read `Entity.Constraints` for lengths.

`AuditableEntityInterceptor` sets `Created`, `CreatedBy`, `LastModified`, and `LastModifiedBy` on save. Leave them unset unless a factory has to stamp `CreatedBy` before save.

Invariants live on the entity: guarded setters, factory methods, and permission methods. Handlers call those instead of reimplementing the rule. Cookbook membership checks live on `CookbookMembership`. `Constants/Roles` and `Constants/Policies` are ASP.NET Identity and are not a cookbook tier.

## Events

Raise events from the entity or the factory (`cookbook.AddDomainEvent(...)`). They dispatch on `SaveChanges` (`DispatchDomainEventsInterceptor`). React in `Application/**/EventHandlers/` with `INotificationHandler<T>`.

## Tests

Pure entity and value-object tests go in `tests/Domain.UnitTests/` (`Entities/`, `ValueObjects/`). No mocks and no database.
