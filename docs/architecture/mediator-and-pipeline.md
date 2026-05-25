# Mediator and request pipeline

Use cases are **commands and queries** dispatched through an in-house mediator in `Application/Common/Mediator/` — **not** the MediatR NuGet package. The API mirrors familiar concepts (`IRequest<T>`, `IRequestHandler<,>`, `IPipelineBehavior<,>`, `Publish`) so the Jason Taylor template shape is preserved without an extra dependency.

Operational detail (folder layout, validators, `DbQuery` pattern) lives in [src/Application/AGENTS.md](../../src/Application/AGENTS.md).

## Send (commands / queries)

```
Endpoint → IMediator.Send(request)
  → IRequestPreProcessor(s)     LoggingBehaviour
  → IPipelineBehavior chain     (outer → inner)
  → IRequestHandler.Handle
```

Behaviors are registered in [DependencyInjection.cs](../../src/Application/DependencyInjection.cs) and wrapped in [Mediator.cs](../../src/Application/Common/Mediator/Mediator.cs) (reversed registration order). **Runtime order:**


| Step | Behavior                      | Role                                               |
| ---- | ----------------------------- | -------------------------------------------------- |
| 1    | `LoggingBehaviour`            | Pre-processor: log request name + user             |
| 2    | `UnhandledExceptionBehaviour` | Log and rethrow                                    |
| 3    | `AuthorizationBehaviour`      | AuthN/AuthZ via attributes on the **request type** |
| 4    | `ValidationBehaviour`         | FluentValidation for the request                   |
| 5    | `PerformanceBehaviour`        | Warn if handler exceeds its threshold (500 ms default; higher via `[LongRunningRequest]` on the request type) |
| 6    | Handler                       | Business logic, usually `IApplicationDbContext`    |


**Authorization** uses custom `[AllowAnonymous]` / `[Authorize]` on the request class ([AuthorizationBehaviour](../../src/Application/Common/Behaviours/AuthorizationBehaviour.cs)), not ASP.NET attributes on handlers. Default: caller must be authenticated (`IUser.Id` set). Endpoint `[RequireAuthorization()]` in Web is a separate gate.

**Void commands** use `IRequest` / `IRequestHandler<T>`; the mediator maps them to `Unit` internally.

## Publish (domain events)

Domain events implement `Domain.Common.INotification`. They are **not** raised from the mediator during `Send`; they are collected on entities and published when EF saves:

1. Handler calls `entity.AddDomainEvent(...)` and `SaveChanges`.
2. [DispatchDomainEventsInterceptor](../../src/Infrastructure/Data/Interceptors/DispatchDomainEventsInterceptor.cs) runs **before** the save, clears events on tracked `BaseEntity` instances, and calls `IMediator.Publish` for each.
3. [EventHandlers/](../../src/Application/) types implement `INotificationHandler<TEvent>` for side effects (logging, notifications, etc.).

Handlers run **in-process** and **before** the transaction commits. Keep event handlers small; heavy work belongs in explicit commands or background jobs if that becomes necessary.

## Where things live


| Concern                 | Location                               |
| ----------------------- | -------------------------------------- |
| Mediator implementation | `Application/Common/Mediator/`         |
| Pipeline behaviors      | `Application/Common/Behaviours/`       |
| Use case handlers       | `Application//Commands`                |
| Domain event reactions  | `Application/<Feature>/EventHandlers/` |
| HTTP entry              | `Web/Endpoints/` → `Send`              |




