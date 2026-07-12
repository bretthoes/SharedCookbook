using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Contracts;

namespace SharedCookbook.Infrastructure.Ai;

public sealed class OpenAiRecipeImageParser : IAiRecipeImageParser
{
    private readonly ChatClient _chatClient;

    private const string SystemPrompt = """
        You are a recipe parser that extracts recipe information from images of recipes (cookbook pages, handwritten cards, screenshots, and printed recipe cards).

        If the image contains a recipe (or partial recipe content), return valid: true with:
        - title (string): The recipe name. If not explicitly shown, infer a concise name from the ingredients and method.
        - summary (string or null): A brief one-sentence description
        - preparationTimeInMinutes, cookingTimeInMinutes, bakingTimeInMinutes, servings (number or null): Only if visible
        - ingredients (array): Each item has "name" (string) and "optional" (boolean). Include quantities and units in the name when visible.
        - directions (array): Each item has "text" (string). May be empty if only ingredients are visible.

        If the image does NOT contain any recognizable food-preparation content (unrelated photo, blank image, scenery, etc.),
        return valid: false with title null, summary null, all time/serving fields null, and empty ingredients and directions arrays.

        Partial content rules (still return valid: true):
        - If only ingredients are visible with no steps, synthesize at least one minimal direction such as "Combine all ingredients and prepare as desired."
        - If only steps are visible with no ingredients, return an empty ingredients array.
        - Infer a title from the content when the recipe is not explicitly named.

        Other rules:
        - Read all visible text, including multi-column layouts and small print.
        - Preserve the recipe's language as written.
        - Handwriting may be imperfect; interpret unclear text as best you can.
        - Mark an ingredient as optional when the image indicates it (e.g. "optional", parentheses, or "if desired").
        - Split run-on directions into separate steps where natural.
        """;

    private const string InvalidContentMessage =
        "The image did not contain enough information to build a recipe.";

    public OpenAiRecipeImageParser(IOptions<AiRecipeParserOptions> options)
    {
        _chatClient = new OpenAIClient(options.Value.ApiKey)
            .GetChatClient(options.Value.Model);
    }

    public async Task<CreateRecipeDto> ParseAsync(IFormFile file, CancellationToken ct = default)
    {
        await using var stream = file.OpenReadStream();
        var imageBytes = await BinaryData.FromStreamAsync(stream, ct);

        List<ChatMessage> messages =
        [
            new SystemChatMessage(SystemPrompt),
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart("Extract the recipe from this image."),
                ChatMessageContentPart.CreateImagePart(imageBytes, GetMediaType(file))),
        ];

        return await OpenAiRecipeParsing.CompleteChatAsync(_chatClient, messages, InvalidContentMessage, ct);
    }

    private static string GetMediaType(IFormFile file)
    {
        if (!string.IsNullOrWhiteSpace(file.ContentType) && file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return file.ContentType;

        return Path.GetExtension(file.FileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".heic" => "image/heic",
            ".heif" => "image/heif",
            ".tiff" or ".tif" => "image/tiff",
            _ => "image/jpeg",
        };
    }
}
