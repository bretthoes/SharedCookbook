using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenAI.Chat;
using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Contracts;

namespace SharedCookbook.Infrastructure.Ai;

internal static class OpenAiRecipeParsing
{
    private const int MaxRetries = 3;

    internal const string RecipeJsonSchema = """
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
                "ingredients",
                "directions"
            ],
            "additionalProperties": false
        }
        """;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private static readonly ChatCompletionOptions CompletionOptions = new()
    {
        ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
            "parsed_recipe",
            BinaryData.FromString(RecipeJsonSchema),
            null,
            true),
        Temperature = 0,
    };

    internal static async Task<CreateRecipeDto> CompleteChatAsync(
        ChatClient chatClient,
        List<ChatMessage> messages,
        string invalidContentMessage,
        CancellationToken ct = default)
    {
        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                var completion = await chatClient.CompleteChatAsync(messages, CompletionOptions, ct);
                return MapToDto(completion.Value.Content[0].Text, invalidContentMessage);
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

        throw new InvalidOperationException("Failed to parse recipe after multiple attempts.");
    }

    private static bool IsTransient(int status) => status is 500 or 502 or 503;

    private static CreateRecipeDto MapToDto(string json, string invalidContentMessage)
    {
        var parsed = JsonSerializer.Deserialize<ParsedRecipe>(json, JsonOptions)
            ?? throw new InvalidOperationException("AI returned an empty or unparseable recipe response.");

        if (!parsed.Valid)
            throw new UnprocessableContentException(invalidContentMessage);

        var ingredients = parsed.Ingredients ?? [];
        var directions = parsed.Directions ?? [];
        var title = string.IsNullOrWhiteSpace(parsed.Title) ? "Untitled Recipe" : parsed.Title.Trim();

        return new CreateRecipeDto
        {
            Title = title,
            Summary = parsed.Summary,
            PreparationTimeInMinutes = parsed.PreparationTimeInMinutes,
            CookingTimeInMinutes = parsed.CookingTimeInMinutes,
            BakingTimeInMinutes = parsed.BakingTimeInMinutes,
            Servings = parsed.Servings,
            IngredientSections = [
                IngredientSectionDto.DefaultWrapper(ingredients
                .Select((ingredient, i) => new RecipeIngredientDto
                {
                    Name = ingredient.Name,
                    Optional = ingredient.Optional,
                    Ordinal = i + 1,
                })
                .ToList())
            ],
            Directions = directions
                .Select((dir, i) => new RecipeDirectionDto
                {
                    Text = dir.Text,
                    Image = null,
                    Ordinal = i + 1,
                })
                .ToList(),
            Images = [],
            CookbookId = Guid.Empty,
        };
    }

    internal sealed record ParsedRecipe(
        [property: JsonPropertyName("valid")] bool Valid,
        string? Title,
        string? Summary,
        int? PreparationTimeInMinutes,
        int? CookingTimeInMinutes,
        int? BakingTimeInMinutes,
        int? Servings,
        [property: JsonPropertyName("ingredients")] List<ParsedIngredient>? Ingredients,
        [property: JsonPropertyName("directions")] List<ParsedDirection>? Directions
    );

    internal sealed record ParsedIngredient(string Name, bool Optional);

    internal sealed record ParsedDirection(string Text);
}
