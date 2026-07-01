using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Contracts;

namespace SharedCookbook.Infrastructure.Ai;

public sealed class OpenAiRecipeEditor : IAiRecipeEditor
{
    private const int MaxRetries = 3;

    private readonly ChatClient _chatClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private const string RecipeEditJsonSchema = """
        {
            "type": "object",
            "properties": {
                "valid": { "type": "boolean" },
                "title": { "type": ["string", "null"] },
                "summary": { "type": ["string", "null"] },
                "preparationTimeInMinutes": { "type": ["integer", "null"] },
                "cookingTimeInMinutes": { "type": ["integer", "null"] },
                "bakingTimeInMinutes": { "type": ["integer", "null"] },
                "servings": { "type": ["integer", "null"] },
                "isVegetarian": { "type": ["boolean", "null"] },
                "isVegan": { "type": ["boolean", "null"] },
                "isGlutenFree": { "type": ["boolean", "null"] },
                "isDairyFree": { "type": ["boolean", "null"] },
                "isHealthy": { "type": ["boolean", "null"] },
                "isCheap": { "type": ["boolean", "null"] },
                "isLowFodmap": { "type": ["boolean", "null"] },
                "isHighProtein": { "type": ["boolean", "null"] },
                "isBreakfast": { "type": ["boolean", "null"] },
                "isLunch": { "type": ["boolean", "null"] },
                "isDinner": { "type": ["boolean", "null"] },
                "isDessert": { "type": ["boolean", "null"] },
                "isSnack": { "type": ["boolean", "null"] },
                "ingredientSections": {
                    "type": "array",
                    "items": {
                        "type": "object",
                        "properties": {
                            "title": { "type": "string" },
                            "ingredients": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                        "name": { "type": "string" },
                                        "optional": { "type": "boolean" }
                                    },
                                    "required": ["name", "optional"],
                                    "additionalProperties": false
                                }
                            }
                        },
                        "required": ["title", "ingredients"],
                        "additionalProperties": false
                    }
                },
                "directions": {
                    "type": "array",
                    "items": {
                        "type": "object",
                        "properties": {
                            "text": { "type": "string" }
                        },
                        "required": ["text"],
                        "additionalProperties": false
                    }
                }
            },
            "required": [
                "valid",
                "title",
                "summary",
                "preparationTimeInMinutes",
                "cookingTimeInMinutes",
                "bakingTimeInMinutes",
                "servings",
                "isVegetarian",
                "isVegan",
                "isGlutenFree",
                "isDairyFree",
                "isHealthy",
                "isCheap",
                "isLowFodmap",
                "isHighProtein",
                "isBreakfast",
                "isLunch",
                "isDinner",
                "isDessert",
                "isSnack",
                "ingredientSections",
                "directions"
            ],
            "additionalProperties": false
        }
        """;

    private const string SystemPrompt = """
        You are a recipe editor. You receive the current recipe as JSON and a user instruction.
        Apply ONLY the changes requested by the user instruction. Preserve everything else unchanged.

        Return valid: true with the full updated recipe when the instruction relates to editing the recipe
        (ingredients, steps, title, summary, times, servings, dietary/meal tags, etc.).

        Return valid: false with all other fields null or empty when the instruction is unrelated to recipe editing
        (e.g. weather, jokes, unrelated questions).

        Rules:
        - Keep the user's language for recipe text unless they ask to translate.
        - ingredientSections: preserve section structure when possible; each section has "title" (string, may be empty) and "ingredients".
        - directions: each item has "text" only.
        - For tag fields (isVegetarian, isVegan, etc.): use true/false when set, null when not applicable or unchanged from context—when the user asks to add/remove a tag, set accordingly.
        - Do not remove ingredients or steps unless the user asks.
        - When adding ingredients or steps, integrate them naturally.
        """;

    public OpenAiRecipeEditor(IOptions<AiRecipeParserOptions> options)
    {
        _chatClient = new OpenAIClient(options.Value.ApiKey)
            .GetChatClient(options.Value.Model);
    }

    public async Task<UpdateRecipeDto> ApplyEditAsync(
        UpdateRecipeDto current,
        string prompt,
        CancellationToken ct = default)
    {
        var currentJson = JsonSerializer.Serialize(ToAiInput(current), JsonOptions);
        var userMessage = $"""
            Current recipe:
            {currentJson}

            User instruction:
            {prompt}
            """;

        List<ChatMessage> messages =
        [
            new SystemChatMessage(SystemPrompt),
            new UserChatMessage(userMessage),
        ];

        var completionOptions = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "edited_recipe",
                BinaryData.FromString(RecipeEditJsonSchema),
                null,
                true),
            Temperature = 0,
        };

        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                var completion = await _chatClient.CompleteChatAsync(messages, completionOptions, ct);
                var json = completion.Value.Content[0].Text;
                return MapToDto(json, current);
            }
            catch (ClientResultException ex) when (ex.Status == 429)
            {
                throw new RateLimitExceededException();
            }
            catch (ClientResultException ex) when (IsTransient(ex.Status))
            {
                if (attempt == MaxRetries - 1)
                    throw;

                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), ct);
            }
            catch (HttpRequestException) when (attempt < MaxRetries - 1)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), ct);
            }
        }

        throw new InvalidOperationException("Failed to apply recipe edit after multiple attempts.");
    }

    private static bool IsTransient(int status) => status is 500 or 502 or 503;

    private static UpdateRecipeDto MapToDto(string json, UpdateRecipeDto current)
    {
        var parsed = JsonSerializer.Deserialize<EditedRecipe>(json, JsonOptions)
            ?? throw new InvalidOperationException("AI returned an empty or unparseable recipe response.");

        if (!parsed.Valid)
            throw new UnprocessableContentException(
                "The instruction could not be applied to this recipe. Try rephrasing your request.");

        var title = string.IsNullOrWhiteSpace(parsed.Title) ? current.Title : parsed.Title.Trim();
        var sections = parsed.IngredientSections ?? [];
        var directions = parsed.Directions ?? [];

        return new UpdateRecipeDto
        {
            Id = current.Id,
            Title = title,
            Summary = parsed.Summary ?? current.Summary,
            PreparationTimeInMinutes = parsed.PreparationTimeInMinutes ?? current.PreparationTimeInMinutes,
            CookingTimeInMinutes = parsed.CookingTimeInMinutes ?? current.CookingTimeInMinutes,
            BakingTimeInMinutes = parsed.BakingTimeInMinutes ?? current.BakingTimeInMinutes,
            Servings = parsed.Servings ?? current.Servings,
            IsVegetarian = parsed.IsVegetarian ?? current.IsVegetarian,
            IsVegan = parsed.IsVegan ?? current.IsVegan,
            IsGlutenFree = parsed.IsGlutenFree ?? current.IsGlutenFree,
            IsDairyFree = parsed.IsDairyFree ?? current.IsDairyFree,
            IsHealthy = parsed.IsHealthy ?? current.IsHealthy,
            IsCheap = parsed.IsCheap ?? current.IsCheap,
            IsLowFodmap = parsed.IsLowFodmap ?? current.IsLowFodmap,
            IsHighProtein = parsed.IsHighProtein ?? current.IsHighProtein,
            IsBreakfast = parsed.IsBreakfast ?? current.IsBreakfast,
            IsLunch = parsed.IsLunch ?? current.IsLunch,
            IsDinner = parsed.IsDinner ?? current.IsDinner,
            IsDessert = parsed.IsDessert ?? current.IsDessert,
            IsSnack = parsed.IsSnack ?? current.IsSnack,
            IngredientSections = sections
                .Select((section, sectionIndex) => new IngredientSectionDto
                {
                    Title = section.Title ?? string.Empty,
                    Ordinal = sectionIndex,
                    Ingredients = (section.Ingredients ?? [])
                        .Select((ingredient, i) => new RecipeIngredientDto
                        {
                            Name = ingredient.Name,
                            Optional = ingredient.Optional,
                            Ordinal = i + 1,
                        })
                        .ToList(),
                })
                .ToList(),
            Directions = directions
                .Select((dir, i) => new RecipeDirectionDto
                {
                    Text = dir.Text,
                    Image = current.Directions.ElementAtOrDefault(i)?.Image,
                    Ordinal = i + 1,
                })
                .ToList(),
            Images = current.Images.ToList(),
        };
    }

    private static object ToAiInput(UpdateRecipeDto recipe) => new
    {
        recipe.Title,
        recipe.Summary,
        recipe.PreparationTimeInMinutes,
        recipe.CookingTimeInMinutes,
        recipe.BakingTimeInMinutes,
        recipe.Servings,
        recipe.IsVegetarian,
        recipe.IsVegan,
        recipe.IsGlutenFree,
        recipe.IsDairyFree,
        recipe.IsHealthy,
        recipe.IsCheap,
        recipe.IsLowFodmap,
        recipe.IsHighProtein,
        recipe.IsBreakfast,
        recipe.IsLunch,
        recipe.IsDinner,
        recipe.IsDessert,
        recipe.IsSnack,
        ingredientSections = recipe.IngredientSections.Select(s => new
        {
            s.Title,
            ingredients = s.Ingredients.Select(i => new { i.Name, i.Optional }),
        }),
        directions = recipe.Directions.Select(d => new { d.Text }),
    };

    private sealed record EditedRecipe(
        [property: JsonPropertyName("valid")] bool Valid,
        string? Title,
        string? Summary,
        int? PreparationTimeInMinutes,
        int? CookingTimeInMinutes,
        int? BakingTimeInMinutes,
        int? Servings,
        bool? IsVegetarian,
        bool? IsVegan,
        bool? IsGlutenFree,
        bool? IsDairyFree,
        bool? IsHealthy,
        bool? IsCheap,
        bool? IsLowFodmap,
        bool? IsHighProtein,
        bool? IsBreakfast,
        bool? IsLunch,
        bool? IsDinner,
        bool? IsDessert,
        bool? IsSnack,
        List<ParsedSection>? IngredientSections,
        List<ParsedDirection>? Directions
    );

    private sealed record ParsedSection(string? Title, List<ParsedIngredient>? Ingredients);

    private sealed record ParsedIngredient(string Name, bool Optional);

    private sealed record ParsedDirection(string Text);
}
