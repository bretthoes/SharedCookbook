# Recipe from URL

The mobile app lets a user paste a link to a recipe on the web (a blog, AllRecipes, etc.). The API fetches structured recipe data from an external service and returns a draft the client can edit before saving.

Implemented with the **Spoonacular** recipe-extract API via [IRecipeUrlParser](../../src/Application/Common/Interfaces/IRecipeUrlParser.cs) (`SpoonacularApiParser`).

## Flow

1. Client sends the recipe page URL (and optionally asks for video extraction).
2. [ParseRecipeFromUrl](../../src/Application/Recipes/Commands/ParseRecipeFromUrl/ParseRecipeFromUrl.cs) delegates to the parser.
3. [SpoonacularApiParser](../../src/Infrastructure/RecipeUrlParser/SpoonacularApiParser.cs) calls Spoonacular’s extract endpoint, deserializes the response, and maps it to `CreateRecipeDto`.
4. If Spoonacular returns an image URL, the parser tries to **re-host** it in our S3 bucket through [IImageUploader](../../src/Application/Common/Interfaces/IImageUploader.cs) (`S3ImageUploader`). Upload failure is logged and ignored so a bad image does not block the import.
5. Client reviews the draft and saves via normal recipe create.

Endpoint: `POST /api/recipes/parse-recipe-url` in [Recipes.cs](../../src/Web/Endpoints/Recipes.cs).

## Where to look in code

| Area | Path |
|------|------|
| HTTP endpoint | [src/Web/Endpoints/Recipes.cs](../../src/Web/Endpoints/Recipes.cs) |
| Command | [src/Application/Recipes/Commands/ParseRecipeFromUrl/](../../src/Application/Recipes/Commands/ParseRecipeFromUrl/) |
| Spoonacular client + mapping | [src/Infrastructure/RecipeUrlParser/](../../src/Infrastructure/RecipeUrlParser/) |
| Config (`ApiKey`, `BaseUrl`) | `RecipeUrlParserOptions` - [DependencyInjection.cs](../../src/Infrastructure/DependencyInjection.cs), secrets in [fly.md](../deploy/fly.md#secrets) |
| Image re-host | [S3ImageUploader](../../src/Infrastructure/FileStorage/S3ImageUploader.cs) |

Direction text from Spoonacular is HTML-decoded and split into steps in [RecipeApiResponseExtensions](../../src/Infrastructure/RecipeUrlParser/Models/RecipeApiResponseExtensions.cs) (paragraph breaks, numbered-step fallback).

## Operational notes

- **Spoonacular API key** is required in every environment; without it, imports fail at the HTTP call.
- **`extractFromVideo`** is passed through to Spoonacular when the client sets it - used for video-heavy recipe sites, not the common case.
- **Costs and quotas** are on Spoonacular’s side; this is the only paid third-party call in the recipe-import family besides OpenAI (voice).
- Spoonacular failures surface as a generic HTTP error to the client; check logs for status code and reason phrase.
