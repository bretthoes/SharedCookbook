using System.Net;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.RecipeUrlParser.Models;

// This class exists to deserialize response from Spoonacular.
// See below for more info:
// https://spoonacular.com/food-api/docs#Extract-Recipe-from-Website
public class RecipeApiResponse
{
    public string? Title { get; init; }
    public string? Summary { get; init; }
    public string? Image { get; set; }
    public int? Servings { get; init; }
    public List<Ingredient>? ExtendedIngredients { get; init; }
    public string? Instructions { get; init; }
    public int? PreparationMinutes { get; init; }
    public int? CookingMinutes { get; init; }

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
        Directions = RecipeApiResponseExtensions.ExtractDirections(Instructions)
    };

    private static string ExtractSummary(string? rawSummary)
    {
        string summary = rawSummary?.RemoveHtml() ?? "";
        string summaryDecoded = WebUtility.HtmlDecode(summary);
        return summaryDecoded.Truncate(Recipe.Constraints.SummaryMaxLength);
    }
}
