using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromImage;

public sealed record ParseRecipeFromImageCommand(IFormFile File) : IRequest<CreateRecipeDto>;

// TODO refactor parsing logic to a separate service
public sealed class ParseRecipeFromImageCommandHandler(IOcrService ocrService)
    : IRequestHandler<ParseRecipeFromImageCommand, CreateRecipeDto>
{
    private static readonly Regex IngredientRegex = new (pattern: @"^- (.+?)( \(optional\))?$", RegexOptions.Multiline);
    
    public async Task<CreateRecipeDto> Handle(
        ParseRecipeFromImageCommand request,
        CancellationToken cancellationToken)
        => ParseRecipeFromText(await ocrService.ExtractText(request.File));

    private static CreateRecipeDto ParseRecipeFromText(string text)
        => CreateRecipeDto(GetTitle(GetLines(text)), ParseIngredients(text), ParseDirections(text));

    private static string[] GetLines(string text) 
        => text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

    private static string GetTitle(string[] lines) 
        => lines.FirstOrDefault()?.Trim() ?? string.Empty;

    private static List<RecipeIngredientDto> ParseIngredients(string text)
    {
        var ingredients = new List<RecipeIngredientDto>();

        int ordinal = 1;
        foreach (Match match in IngredientRegex.Matches(text))
        {
            ingredients.Add(new RecipeIngredientDto
            {
                Name = match.Groups[1].Value,
                Optional = match.Groups[2].Success,
                Ordinal = ordinal++
            });
        }

        return ingredients;
    }

    private static List<RecipeDirectionDto> ParseDirections(string text)
    {
        var directions = new List<RecipeDirectionDto>();
    
        string[] lines = text.Split('\n')
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        bool isDirections = false;
        foreach (string line in lines)
        {
            if (line.StartsWith("Directions", StringComparison.OrdinalIgnoreCase) || 
                line.StartsWith("Steps", StringComparison.OrdinalIgnoreCase) || 
                line.StartsWith("Instructions", StringComparison.OrdinalIgnoreCase))
            {
                isDirections = true;
                continue;
            }

            if (isDirections)
            {
                directions.Add(new RecipeDirectionDto
                {
                    Text = line,
                    Image = null,
                    Ordinal = directions.Count + 1
                });
            }
        }

        return directions;
    }

    private static CreateRecipeDto CreateRecipeDto(
        string title,
        List<RecipeIngredientDto> ingredients,
        List<RecipeDirectionDto> directions) => new()
    {
        Title = title,
        Summary = null,
        PreparationTimeInMinutes = null,
        CookingTimeInMinutes = null,
        BakingTimeInMinutes = null,
        Servings = null,
        Ingredients = ingredients,
        Directions = directions,
        Images = [],
        CookbookId = 0
    };
}
