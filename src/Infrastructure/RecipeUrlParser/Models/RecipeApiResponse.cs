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
    public string? Image { get; init; }
    public int? Servings { get; init; }
    public List<Ingredient>? ExtendedIngredients { get; init; }
    public string? Instructions { get; init; }
    public List<AnalyzedInstruction>? AnalyzedInstructions { get; init; }
    public int? PreparationMinutes { get; init; }
    public int? CookingMinutes { get; init; }

    public bool HasImage() => Image is not null && Image.IsValidUrl();

    public CreateRecipeDto ToDto(string? imageKey) => new()
    {
        Title = Title?.Truncate(Recipe.Constraints.TitleMaxLength) ?? "",
        Images = string.IsNullOrWhiteSpace(imageKey)
            ? []
            : [new RecipeImageDto { Name = imageKey, Ordinal = 1 }],
        CookbookId = Guid.Empty,
        Summary = ExtractSummary(Summary),
        Servings = Servings ?? 0,
        PreparationTimeInMinutes = PreparationMinutes ?? 0,
        CookingTimeInMinutes = CookingMinutes ?? 0,
        BakingTimeInMinutes = null,
        IngredientSections = [IngredientSectionDto.DefaultWrapper(ExtendedIngredients.ToDtos())],
        Directions = ShouldUseAnalyzedInstructions()
            ? AnalyzedInstructionDirectionsParser.Parse(AnalyzedInstructions)
            : RawInstructionDirectionsParser.Parse(Instructions)
    };

    private bool ShouldUseAnalyzedInstructions() => AnalyzedInstructions is { Count: > 0 };

    private static string ExtractSummary(string? rawSummary)
    {
        string summary = rawSummary?.RemoveHtml() ?? "";
        string summaryDecoded = WebUtility.HtmlDecode(summary);
        return summaryDecoded.Truncate(Recipe.Constraints.SummaryMaxLength);
    }
}

