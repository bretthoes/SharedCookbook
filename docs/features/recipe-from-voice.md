# Recipe from voice

The mobile app records the user describing a recipe (speech-to-text on the device), then sends the **transcript** to the API. The API turns that text into a structured recipe draft the client can edit before saving.

**Speech recognition is not in this backend** — only parsing the transcript. There is no Whisper or other STT service in SharedCookbook.

Implemented with **OpenAI Chat Completions** via [IAiRecipeParser](../../src/Application/Common/Interfaces/IAiRecipeParser.cs) (`OpenAiRecipeParser`).

## Flow

1. Client obtains a transcript locally (platform speech APIs).
2. [ParseRecipeFromVoice](../../src/Application/Recipes/Commands/ParseRecipeFromVoice/ParseRecipeFromVoice.cs) sends the transcript to `OpenAiRecipeParser`.
3. The parser uses a fixed system prompt and JSON response format to produce title, ingredients, directions, and optional times/servings.
4. If the model decides the input is not a recipe, parsing fails with a clear error (no draft).
5. Client reviews and saves via normal recipe create.

Endpoint: `POST /api/recipes/parse-recipe-voice` in [Recipes.cs](../../src/Web/Endpoints/Recipes.cs).

## Where to look in code

| Area | Path |
|------|------|
| HTTP endpoint | [src/Web/Endpoints/Recipes.cs](../../src/Web/Endpoints/Recipes.cs) |
| Command | [src/Application/Recipes/Commands/ParseRecipeFromVoice/](../../src/Application/Recipes/Commands/ParseRecipeFromVoice/) |
| OpenAI client + prompt | [OpenAiRecipeParser.cs](../../src/Infrastructure/Ai/OpenAiRecipeParser.cs) |
| Config (`ApiKey`, `Model`) | `AiRecipeParserOptions` — default model `gpt-4o-mini`; [DependencyInjection.cs](../../src/Infrastructure/DependencyInjection.cs), secrets in [fly.md](../deploy/fly.md#secrets) |
| Rate limit → 429 | `RateLimitExceededException` in parser; handled in [CustomExceptionHandler.cs](../../src/Web/Infrastructure/CustomExceptionHandler.cs) |

The system prompt and mapping logic live entirely in `OpenAiRecipeParser` — that is the place to change behavior (e.g. stricter “is this a recipe?” checks).

## Operational notes

- **OpenAI API key** required; model name is configurable per environment.
- **429 from OpenAI** is translated to “too many requests” for the client — distinct from the app’s own [rate limiter](../../src/Web/Infrastructure/RateLimiterExtension.cs) on HTTP.
- Same pattern as URL and photo import: returns `CreateRecipeDto`, not a persisted recipe.

## Related imports

Photo and URL import are documented in [recipe-from-photo.md](./recipe-from-photo.md) and [recipe-from-url.md](./recipe-from-url.md). All three are draft-only endpoints under `/api/recipes/parse-recipe-*`.
