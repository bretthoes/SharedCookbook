# Recipe from photo

The mobile app lets a user take a picture of a recipe (a cookbook page, a handwritten card, a screenshot). The API reads the image with OpenAI vision, parses it into a recipe shape, and returns a draft the client can edit before saving.

Implemented with **OpenAI Chat Completions (vision)** via [IAiRecipeImageParser](../../src/Application/Common/Interfaces/IAiRecipeImageParser.cs) (`OpenAiRecipeImageParser`). Uses the same `AiRecipeParserOptions` config and JSON schema as [recipe-from-voice.md](./recipe-from-voice.md).

## API and application flow

### Endpoint `POST /api/recipes/parse-recipe-img`

[src/Web/Endpoints/Recipes.cs](../../src/Web/Endpoints/Recipes.cs)

```csharp
builder.MapPost(ParseFromImage, pattern: "/parse-recipe-img").RequireAuthorization();
// ...
private static Task<CreateRecipeDto> ParseFromImage(ISender sender, [FromForm] IFormFile file)
    => sender.Send(new ParseRecipeFromImageCommand(file));
```

### Command + handler

[src/Application/Recipes/Commands/ParseRecipeFromImage/ParseRecipeFromImage.cs](../../src/Application/Recipes/Commands/ParseRecipeFromImage/ParseRecipeFromImage.cs)

The handler sends the uploaded image to `OpenAiRecipeImageParser`, which returns a structured `CreateRecipeDto` (title, ingredients, directions, optional times/servings). If the model decides the image is not a recipe, parsing fails with a clear error (no draft).

Validation: [ParseRecipeFromImageCommandValidator.cs](../../src/Application/Recipes/Commands/ParseRecipeFromImage/ParseRecipeFromImageCommandValidator.cs) — max 10 MB, allowed image extensions.

## Vision implementation

| Area | Path |
|------|------|
| HTTP endpoint | [src/Web/Endpoints/Recipes.cs](../../src/Web/Endpoints/Recipes.cs) |
| Command | [src/Application/Recipes/Commands/ParseRecipeFromImage/](../../src/Application/Recipes/Commands/ParseRecipeFromImage/) |
| OpenAI client + prompt | [OpenAiRecipeImageParser.cs](../../src/Infrastructure/Ai/OpenAiRecipeImageParser.cs) |
| Shared JSON schema + mapping | [OpenAiRecipeParsing.cs](../../src/Infrastructure/Ai/OpenAiRecipeParsing.cs) |
| Config (`ApiKey`, `Model`) | `AiRecipeParserOptions` — default model `gpt-4o-mini` (vision-capable); [DependencyInjection.cs](../../src/Infrastructure/DependencyInjection.cs), secrets in [fly.md](../deploy/fly.md#secrets) |
| Rate limit → 429 | `RateLimitExceededException` in parser; handled in [CustomExceptionHandler.cs](../../src/Web/Infrastructure/CustomExceptionHandler.cs) |

The image parser uses a vision-specific system prompt. Shared retry logic, JSON schema, and DTO mapping live in `OpenAiRecipeParsing` alongside the voice parser.

## Operational notes

- **OpenAI API key** required; same key/model as voice parsing.
- **429 from OpenAI** is translated to “too many requests” for the client — distinct from the app’s own [rate limiter](../../src/Web/Infrastructure/RateLimiterExtension.cs) on HTTP.
- No native OCR dependencies in the Docker image (Tesseract/Leptonica removed).
- Default model `gpt-4o-mini` supports vision; use a vision-capable model if you override `AiRecipeParserOptions__Model`.

## Related imports

Voice and URL import are documented in [recipe-from-voice.md](./recipe-from-voice.md) and [recipe-from-url.md](./recipe-from-url.md). All three are draft-only endpoints under `/api/recipes/parse-recipe-*`.
