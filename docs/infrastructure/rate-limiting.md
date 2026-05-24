# Rate limiting

Protects the API from abuse and caps usage with external cost (third-party APIs, S3). Code: [src/Web/Infrastructure/RateLimiting/](../../src/Web/Infrastructure/RateLimiting/). Wired in [RateLimiterExtension.cs](../../src/Web/Infrastructure/RateLimiterExtension.cs); middleware in [Program.cs](../../src/Web/Program.cs).

## Global limiter

Every request. [GlobalRequestRateLimiter.cs](../../src/Web/Infrastructure/RateLimiting/GlobalRequestRateLimiter.cs)

| Window | Limit |
|--------|-------|
| 10 seconds | 30 requests |
| 1 minute | 120 requests |

Both must pass (chained). Hard-coded, not configurable.

**Partition:** authenticated user id → client IP → `"anonymous"`. See [RateLimitPartitionKeys.cs](../../src/Web/Infrastructure/RateLimiting/RateLimitPartitionKeys.cs).

## Daily policies

24-hour fixed window per partition. Default **100 requests/day** each. Separate counters — parse limits don't affect uploads and vice versa.

### Recipe parsing (`RecipeParsingDaily`)

Shared quota across:

| Endpoint | Cost driver |
|----------|-------------|
| `POST /api/recipes/parse-recipe-url` | Spoonacular |
| `POST /api/recipes/parse-recipe-img` | OCR + downstream |
| `POST /api/recipes/parse-recipe-voice` | OpenAI |

[Recipes.cs](../../src/Web/Endpoints/Recipes.cs) · [RecipeParsingDailyRateLimiterPolicy.cs](../../src/Web/Infrastructure/RateLimiting/RecipeParsingDailyRateLimiterPolicy.cs)

### Image upload (`ImageUploadDaily`)

| Endpoint | Cost driver |
|----------|-------------|
| `POST /api/Images` | S3 |

One permit per HTTP request (multi-file uploads count once). [Images.cs](../../src/Web/Endpoints/Images.cs) · [ImageUploadDailyRateLimiterPolicy.cs](../../src/Web/Infrastructure/RateLimiting/ImageUploadDailyRateLimiterPolicy.cs)

## Configuration

Defaults apply when absent. Override per environment as needed.

```json
{
  "RecipeParsingRateLimit": { "DailyLimit": 100 },
  "ImageUploadRateLimit": { "DailyLimit": 100 }
}
```

## 429 responses

Middleware rejections: plain text `Too many requests. Please try again later.`, optional `Retry-After` header. [RateLimiterRejectionHandler.cs](../../src/Web/Infrastructure/RateLimiting/RateLimiterRejectionHandler.cs)

**Not the same** as OpenAI's own rate limits — those throw [RateLimitExceededException](../../src/Application/Common/Exceptions/RateLimitExceededException.cs) and return JSON `ProblemDetails`.

## Adding a policy

1. Constant in [RateLimitPolicyNames.cs](../../src/Web/Infrastructure/RateLimiting/RateLimitPolicyNames.cs)
2. `IRateLimiterPolicy<string>` — reuse [DailyClientRateLimitPartitions.cs](../../src/Web/Infrastructure/RateLimiting/DailyClientRateLimitPartitions.cs) for daily per-client quotas
3. Register in [RateLimiterExtension.cs](../../src/Web/Infrastructure/RateLimiterExtension.cs)
4. `.RequireRateLimiting(...)` on the endpoint
