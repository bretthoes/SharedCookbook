# Exception handling

How application errors become HTTP responses. Implementation: [CustomExceptionHandler.cs](../../src/Web/Infrastructure/CustomExceptionHandler.cs).

## Request flow

```
Client
  → auth / rate limiter / static files / swagger   (Program.cs middleware)
  → minimal API endpoint                           (Web/Endpoints/)
  → IMediator.Send(command or query)
  → pipeline behaviours (log, authorize, validate, …)
  → handler                                        (throws on failure)
  → exception bubbles up
  → CustomExceptionHandler                         (JSON ProblemDetails)
```

Handlers and validators do **not** set status codes themselves. They throw typed exceptions from `Application/Common/Exceptions/`; the Web layer maps those to HTTP at the edge.

The mediator's `UnhandledExceptionBehaviour` only **logs and rethrows** — it does not translate errors. `ValidationBehaviour` turns FluentValidation failures into `ValidationException`.

ASP.NET model-state validation is turned off (`SuppressModelStateInvalidFilter`); input rules live in FluentValidation instead.

## Where it sits in the pipeline

Registered in `AddWebServices()` → `AddExceptionHandler<CustomExceptionHandler>()`.

In [Program.cs](../../src/Web/Program.cs), `UseExceptionHandler` runs **after** authentication, authorization, and rate limiting, and **before** endpoints are invoked. Exceptions thrown during handler execution are caught here and turned into JSON responses.

Anything the handler does not recognize falls through to ASP.NET's default exception handling (typically a 500 in production).

## Mapped exceptions

| Thrown when | HTTP | Response shape |
|-------------|------|----------------|
| Validation failed (FluentValidation) | 400, 413, or 415 | `ValidationProblemDetails` (field errors) for 400; `ProblemDetails` for 413/415 |
| Entity not found | 404 | `ProblemDetails` with message |
| Not authenticated | 401 | `ProblemDetails` |
| Authenticated but not allowed | 403 | `ProblemDetails` |
| Conflicting state (e.g. duplicate) | 409 | `ProblemDetails` |
| External provider rate limit (e.g. OpenAI 429) | 429 | `ProblemDetails` with message |
| Valid request but unusable input (e.g. voice transcript not a recipe) | 422 | `ProblemDetails` with message |

All mapped responses are JSON (`application/problem+json`).

## Not the same as HTTP rate limiting

The ASP.NET rate limiter rejects requests **before** they reach a handler — plain-text 429, not `ProblemDetails`. See [rate-limiting.md](./rate-limiting.md).

`RateLimitExceededException` is different: the request reached the handler, called an external API, and **that** provider returned 429. The handler translates it to JSON `ProblemDetails`.

## Adding a new error type

1. Add a small exception class under `Application/Common/Exceptions/`.
2. Throw it from the handler (or infrastructure service the handler calls).
3. Register a handler in `CustomExceptionHandler` that sets the status code and writes `ProblemDetails`.

If the mobile client needs to branch on the error, pick a distinct status code (like 422) or a stable `title`/`detail` string — don't rely on undocumented 500 bodies.
