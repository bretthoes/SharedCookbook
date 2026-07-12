using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Contracts;

namespace SharedCookbook.Infrastructure.Ai;

public sealed class OpenAiRecipeParser : IAiRecipeParser
{
    private readonly ChatClient _chatClient;

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

    private const string InvalidContentMessage =
        "The transcript did not contain enough information to build a recipe.";

    public OpenAiRecipeParser(IOptions<AiRecipeParserOptions> options)
    {
        _chatClient = new OpenAIClient(options.Value.ApiKey)
            .GetChatClient(options.Value.Model);
    }

    public Task<CreateRecipeDto> ParseAsync(string transcript, CancellationToken ct = default)
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

        return OpenAiRecipeParsing.CompleteChatAsync(_chatClient, messages, InvalidContentMessage, ct);
    }
}
