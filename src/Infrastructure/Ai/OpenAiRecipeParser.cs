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
    private readonly ChatClient _chatClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private const string SystemPrompt = """
        You are a recipe parser that converts spoken recipe descriptions into structured JSON.

        If the input describes a recipe, return a JSON object with exactly these fields:
        - valid (boolean, always true for a recognised recipe)
        - title (string, required): The recipe name. If not explicitly stated, infer a concise name from the ingredients and method.
        - summary (string or null): A brief one-sentence description
        - preparationTimeInMinutes (number or null): Prep time if mentioned
        - cookingTimeInMinutes (number or null): Active cook time if mentioned
        - bakingTimeInMinutes (number or null): Oven/bake time if mentioned
        - servings (number or null): Number of servings if mentioned
        - ingredients (array, required, at least one item): Each item has "name" (string) and "optional" (boolean)
        - directions (array, required, at least one item): Each item has "text" (string) representing one step

        If the input does NOT describe a recipe (e.g. it is random speech, silence, unrelated text, or lacks
        both ingredients and steps), return exactly: { "valid": false }

        Rules:
        - Extract all ingredients and steps, preserving the speaker's language.
        - Mark an ingredient as optional if the speaker says "optional", "if you have", "you can add", or similar.
        - Split run-on directions into separate steps where natural.
        - Return ONLY valid JSON. No markdown fences, no extra commentary.
        """;

    public OpenAiRecipeParser(IOptions<AiRecipeParserOptions> options)
    {
        _chatClient = new OpenAIClient(options.Value.ApiKey)
            .GetChatClient(options.Value.Model);
    }

    public async Task<CreateRecipeDto> ParseAsync(string transcript, CancellationToken cancellationToken)
    {
        List<ChatMessage> messages =
        [
            new SystemChatMessage(SystemPrompt),
            new UserChatMessage(transcript),
        ];

        var completionOptions = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
        };

        try
        {
            var completion = await _chatClient.CompleteChatAsync(messages, completionOptions, cancellationToken);
            var json = completion.Value.Content[0].Text;
            return MapToDto(json);
        }
        catch (ClientResultException ex) when (ex.Status == 429)
        {
            throw new RateLimitExceededException();
        }
    }

    private static CreateRecipeDto MapToDto(string json)
    {
        var parsed = JsonSerializer.Deserialize<ParsedRecipe>(json, JsonOptions)
            ?? throw new InvalidOperationException("AI returned an empty or unparseable recipe response.");

        if (!parsed.Valid)
            throw new InvalidOperationException("The transcript did not contain enough information to build a recipe.");

        return new CreateRecipeDto
        {
            Title = parsed.Title,
            Summary = parsed.Summary,
            PreparationTimeInMinutes = parsed.PreparationTimeInMinutes,
            CookingTimeInMinutes = parsed.CookingTimeInMinutes,
            BakingTimeInMinutes = parsed.BakingTimeInMinutes,
            Servings = parsed.Servings,
            Ingredients = parsed.Ingredients
                .Select((ing, i) => new RecipeIngredientDto
                {
                    Name = ing.Name,
                    Optional = ing.Optional,
                    Ordinal = i + 1,
                })
                .ToList(),
            Directions = parsed.Directions
                .Select((dir, i) => new RecipeDirectionDto
                {
                    Text = dir.Text,
                    Image = null,
                    Ordinal = i + 1,
                })
                .ToList(),
            Images = [],
            CookbookId = 0,
        };
    }

    private sealed record ParsedRecipe(
        [property: JsonPropertyName("valid")] bool Valid,
        string Title,
        string? Summary,
        int? PreparationTimeInMinutes,
        int? CookingTimeInMinutes,
        int? BakingTimeInMinutes,
        int? Servings,
        [property: JsonPropertyName("ingredients")] List<ParsedIngredient> Ingredients,
        [property: JsonPropertyName("directions")] List<ParsedDirection> Directions
    );

    private sealed record ParsedIngredient(string Name, bool Optional);

    private sealed record ParsedDirection(string Text);
}
