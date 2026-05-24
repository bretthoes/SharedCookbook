# Subscriptions (Pro tier)

Users can subscribe to a **Pro** plan to remove the free-tier daily parse quota. Purchases are processed client-side via RevenueCat; the backend only needs to receive the resulting webhook and issue the correct JWT claim.

## Tier limits

| Tier | Daily parse quota |
|------|------------------|
| Free | 15 |
| Pro | 100 |

The parse quota applies to `RecipeParsingDaily` (URL, photo, voice endpoints). See [rate-limiting.md](../infrastructure/rate-limiting.md).

## Endpoints

| Endpoint | Auth | Purpose |
|----------|------|---------|
| `GET /api/Subscriptions/status` | Bearer | Returns `{ isPro: bool }` read from the `subscription_tier` JWT claim — no DB round-trip |
| `POST /api/Subscriptions/webhook/revenuecat` | shared secret | Receives RevenueCat lifecycle events and updates `ApplicationUser.SubscriptionTier` |

Source: [src/Web/Endpoints/Subscriptions.cs](../../src/Web/Endpoints/Subscriptions.cs)

## Webhook flow

1. RevenueCat fires `POST /api/Subscriptions/webhook/revenuecat` after a purchase or lifecycle change.
2. The handler validates the `Authorization` header against `RevenueCat:SharedSecret`.
3. `event.app_user_id` is resolved to an `ApplicationUser` via `UserManager.FindByIdAsync`.
4. The event type is mapped to a new `SubscriptionTier` (see table below) and persisted.
5. On the next token issue/refresh, `SubscriptionClaimsPrincipalFactory` includes a `subscription_tier` claim; the rate-limit policy then applies the correct ceiling.

| RevenueCat event type | Resulting tier |
|-----------------------|---------------|
| `INITIAL_PURCHASE`, `RENEWAL`, `UNCANCELLATION`, `PRODUCT_CHANGE`, `TRANSFER`, `TRIAL_CONVERTED` | Pro |
| `EXPIRATION` | Free |
| `CANCELLATION` | *(no change — subscription still active until expiration)* |
| All others | *(no change)* |

## Where to look in code

| Area | Path |
|------|------|
| `SubscriptionTier` enum | [src/Infrastructure/Identity/SubscriptionTier.cs](../../src/Infrastructure/Identity/SubscriptionTier.cs) |
| `subscription_tier` claim constant | [src/Infrastructure/Identity/SubscriptionClaims.cs](../../src/Infrastructure/Identity/SubscriptionClaims.cs) |
| Custom claims factory | [src/Infrastructure/Identity/SubscriptionClaimsPrincipalFactory.cs](../../src/Infrastructure/Identity/SubscriptionClaimsPrincipalFactory.cs) |
| Endpoints | [src/Web/Endpoints/Subscriptions.cs](../../src/Web/Endpoints/Subscriptions.cs) |
| Webhook options | [src/Web/Infrastructure/RateLimiting/RevenueCatWebhookOptions.cs](../../src/Web/Infrastructure/RateLimiting/RevenueCatWebhookOptions.cs) |
| EF migration | [src/Infrastructure/Data/Migrations/00000000000017_AddSubscriptionTierToUser.cs](../../src/Infrastructure/Data/Migrations/00000000000017_AddSubscriptionTierToUser.cs) |

## Configuration

```json
{
  "RevenueCat": {
    "SharedSecret": "<Authorization header value from RevenueCat dashboard>"
  },
  "RecipeParsingRateLimit": {
    "DailyLimit": 100,
    "FreeDailyLimit": 15
  }
}
```

Leave `SharedSecret` empty to skip webhook authentication in development.

## Operational notes

- The handler is idempotent — writing the same tier twice is safe. RevenueCat retries on non-2xx with exponential back-off for up to 5 days.
- If `app_user_id` does not match any `ApplicationUser`, the webhook returns 200 and logs a warning (does not fail the delivery).
- `CANCELLATION` is a no-op here because the subscription stays active until billing period end; `EXPIRATION` is the event that triggers the downgrade.
- Rate-limit partition keys are `userId:Free` and `userId:Pro`. An upgrade takes effect on the next request without waiting for an in-memory window to expire.
