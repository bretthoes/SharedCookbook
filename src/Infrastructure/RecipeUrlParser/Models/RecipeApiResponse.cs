using System.Net;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.RecipeUrlParser.Models;

// This class exists to deserialize response from Spoonacular.
// See below for more info:
// https://spoonacular.com/food-api/docs#Extract-Recipe-from-Website
public class RecipeApiResponse
{
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public string? Image { get; set; }
    public int? Servings { get; set; }
    public List<Ingredient>? ExtendedIngredients { get; set; }
    public string? Instructions { get; set; }
    public int? PreparationMinutes { get; set; }
    public int? CookingMinutes { get; set; }

    public bool HasImage() => Image is not null && Image.IsValidUrl();

    public CreateRecipeDto ToDto() => new()
    {
        Title = Title?.Truncate(Recipe.Constraints.TitleMaxLength) ?? "",
        Images = string.IsNullOrWhiteSpace(Image)
            ? []
            : [new RecipeImageDto { Name = Image, Ordinal = 1 }],
        CookbookId = 0,
        Summary = ExtractSummary(Summary),
        Servings = Servings ?? 0,
        PreparationTimeInMinutes = PreparationMinutes ?? 0,
        CookingTimeInMinutes = CookingMinutes ?? 0,
        BakingTimeInMinutes = null,
        Ingredients = ExtendedIngredients.ToDtos(),
        Directions = ExtractDirections(Instructions)
    };

    private static string ExtractSummary(string? rawSummary)
    {
        string summary = rawSummary?.RemoveHtml() ?? "";
        string summaryDecoded = WebUtility.HtmlDecode(summary);
        return summaryDecoded.Truncate(Recipe.Constraints.SummaryMaxLength);
    }

    // Split instructions into individual steps based on double newline characters
    private static List<RecipeDirectionDto> ExtractDirections(string? rawDirections)
        => string.IsNullOrWhiteSpace(rawDirections)
            ? []
            : WebUtility.HtmlDecode(rawDirections)
                .Split(["\n\n"], StringSplitOptions.RemoveEmptyEntries)
                .Select(stepString => stepString.RemoveHtml().Trim())
                .Where(stepString => stepString.Length > 0)
                .AsEnumerable()
                .ToDtos();
}

internal static class RecipeUrlParserExtensions
{
    internal static List<RecipeDirectionDto> ToDtos(this IEnumerable<string>? directions) =>
        directions is null
            ? []
            : directions.Select((direction, index) => new RecipeDirectionDto
            {
                Text = direction.Truncate(RecipeDirection.Constraints.TextMaxLength), Ordinal = index + 1
            }).ToList();
}
