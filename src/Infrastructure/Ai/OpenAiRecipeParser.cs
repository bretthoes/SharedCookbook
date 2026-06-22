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

public sealed class OpenAiRecipeParser : IAiRecipeParser
{
    private const int MaxRetries = 3;

    private readonly ChatClient _chatClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private const string RecipeJsonSchema = """
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

    private const string SystemPrompt = """
        You are a recipe parser that converts spoken recipe descriptions into structured JSON.

        If the input describes a recipe (or partial recipe content), return valid: true with:
        - title (string): The recipe name. If not explicitly stated, infer a concise name from the ingredients and method.
        - summary (string or null): A brief one-sentence description
        - preparationTimeInMinutes, cookingTimeInMinutes, bakingTimeInMinutes, servings (number or null): Only if mentioned
        - ingredients (array): Each item has "name" (string) and "optional" (boolean). May be empty if only steps were spoken.
        - directions (array): Each item has "text" (string). May be empty if only ingredients were spoken.

        If the input does NOT contain any recognizable food-preparation content (random speech, silence, unrelated text),
        return valid: false with title null, summary null, all time/serving fields null, and empty ingredients and directions arrays.

        Partial content rules (still return valid: true):
        - If only ingredients are listed with no steps, synthesize at least one minimal direction such as "Combine all ingredients and prepare as desired."
        - If only steps are listed with no ingredients, return an empty ingredients array.
        - Infer a title from the content when the speaker does not name the dish.

        Other rules:
        - Extract all ingredients and steps, preserving the speaker's language.
        - Mark an ingredient as optional if the speaker says "optional", "if you have", "you can add", or similar.
        - Split run-on directions into separate steps where natural.
        """;

    private const string FewShotTranscript = """
        This is my grandma's banana bread. You'll need two cups of flour, three ripe bananas,
        one cup of sugar, two eggs, and half a cup of butter. Preheat the oven to 350 degrees.
        Mash the bananas, mix in the wet ingredients, then fold in the flour and sugar.
        Bake for about 55 minutes until a toothpick comes out clean.
        """;

    private const string FewShotResponse = """
        {"valid":true,"title":"Grandma's Banana Bread","summary":"A classic moist banana bread baked at 350 degrees.","preparationTimeInMinutes":null,"cookingTimeInMinutes":null,"bakingTimeInMinutes":55,"servings":null,"ingredients":[{"name":"2 cups flour","optional":false},{"name":"3 ripe bananas","optional":false},{"name":"1 cup sugar","optional":false},{"name":"2 eggs","optional":false},{"name":"1/2 cup butter","optional":false}],"directions":[{"text":"Preheat the oven to 350 degrees."},{"text":"Mash the bananas, mix in the wet ingredients, then fold in the flour and sugar."},{"text":"Bake for about 55 minutes until a toothpick comes out clean."}]}
        """;

    private const string FewShotInvalidTranscript = "Hey what's the weather like tomorrow I think it's going to rain.";

    private const string FewShotInvalidResponse = """
        {"valid":false,"title":null,"summary":null,"preparationTimeInMinutes":null,"cookingTimeInMinutes":null,"bakingTimeInMinutes":null,"servings":null,"ingredients":[],"directions":[]}
        """;

    public OpenAiRecipeParser(IOptions<AiRecipeParserOptions> options)
    {
        _chatClient = new OpenAIClient(options.Value.ApiKey)
            .GetChatClient(options.Value.Model);
    }

    public async Task<CreateRecipeDto> ParseAsync(string transcript, CancellationToken ct = default)
    {
        List<ChatMessage> messages =
        [
            new SystemChatMessage(SystemPrompt),
            new UserChatMessage(FewShotTranscript),
            new AssistantChatMessage(FewShotResponse),
            new UserChatMessage(FewShotInvalidTranscript),
            new AssistantChatMessage(FewShotInvalidResponse),
            new UserChatMessage(transcript),
        ];

        var completionOptions = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "parsed_recipe",
                BinaryData.FromString(RecipeJsonSchema),
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
                return MapToDto(json);
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

    private static CreateRecipeDto MapToDto(string json)
    {
        var parsed = JsonSerializer.Deserialize<ParsedRecipe>(json, JsonOptions)
            ?? throw new InvalidOperationException("AI returned an empty or unparseable recipe response.");

        if (!parsed.Valid)
            throw new UnprocessableContentException("The transcript did not contain enough information to build a recipe.");

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

    private sealed record ParsedRecipe(
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

    private sealed record ParsedIngredient(string Name, bool Optional);

    private sealed record ParsedDirection(string Text);
}
