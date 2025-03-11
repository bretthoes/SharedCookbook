using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;

namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromImage;

public record ParseRecipeFromImageCommand(IFormFile File) : IRequest<CreateRecipeDto>;

public partial class ParseRecipeFromImageCommandHandler(IOcrService ocrService)
    : IRequestHandler<ParseRecipeFromImageCommand, CreateRecipeDto>
{
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

    private static List<CreateRecipeIngredientDto> ParseIngredients(string text)
    {
        var ingredients = new List<CreateRecipeIngredientDto>();
        var ingredientRegex = MyRegex();

        int ordinal = 1;
        foreach (Match match in ingredientRegex.Matches(text))
        {
            ingredients.Add(new CreateRecipeIngredientDto
            {
                Name = match.Groups[1].Value,
                Optional = match.Groups[2].Success,
                Ordinal = ordinal++
            });
        }

        return ingredients;
    }

    private static List<CreateRecipeDirectionDto> ParseDirections(string text)
    {
        var directions = new List<CreateRecipeDirectionDto>();
    
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
                directions.Add(new CreateRecipeDirectionDto
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
        List<CreateRecipeIngredientDto> ingredients,
        List<CreateRecipeDirectionDto> directions) => new()
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
    [GeneratedRegex(@"^- (.+?)( \(optional\))?$", RegexOptions.Multiline)]
    private static partial Regex MyRegex();
}
