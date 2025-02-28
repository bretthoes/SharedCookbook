using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace SharedCookbook.Application.Images.Commands.ParseRecipe;

public record ParseRecipeCommand(IFormFile File) : IRequest<RecipeFormInputs>;

public class ParseRecipeCommandHandler : IRequestHandler<ParseRecipeCommand, RecipeFormInputs>
{
    private readonly IOcrService _ocrService;

    public ParseRecipeCommandHandler(IOcrService ocrService)
    {
        _ocrService = ocrService;
    }

    public async Task<RecipeFormInputs> Handle(ParseRecipeCommand request, CancellationToken cancellationToken)
    {
        string extractedText = await _ocrService.ExtractText(request.File);

        return ParseRecipeFromText(extractedText);
    }
    
    private static RecipeFormInputs ParseRecipeFromText(string text)
    {
        var ingredients = new List<RecipeFormInputs.Ingredient>();
        var directions = new List<RecipeFormInputs.Direction>();

        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string title = lines.FirstOrDefault()?.Trim() ?? "Unknown Recipe";

        // Extract Ingredients (Lines with - or *)
        var ingredientRegex = new Regex(@"^- (.+?)( \(optional\))?$", RegexOptions.Multiline);
        foreach (Match match in ingredientRegex.Matches(text))
        {
            ingredients.Add(new RecipeFormInputs.Ingredient
            {
                Name = match.Groups[1].Value,
                Optional = match.Groups[2].Success
            });
        }

        // Detect Directions Section (Handles "Steps:", "Directions:", "Instructions:", etc.)
        var directionsRegex = new Regex(@"^(Steps|Directions|Instructions):",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);
        var directionsMatch = directionsRegex.Match(text);

        if (!directionsMatch.Success)
        {
            return new RecipeFormInputs
            {
                Title = title,
                Summary = null,
                PreparationTimeInMinutes = null,
                CookingTimeInMinutes = null,
                BakingTimeInMinutes = null,
                Servings = null,
                Ingredients = ingredients,
                Directions = directions,
                Images = []
            };
        }

        int startIndex = directionsMatch.Index;
        var directionsLines = text[startIndex..].Split('\n').Skip(1); // Skip the header line

        directions.AddRange(from line in directionsLines
            select line.Trim()
            into step
            where !string.IsNullOrWhiteSpace(step)
            select new RecipeFormInputs.Direction
            {
                Text = step, Image = null // No image parsing yet
            });

        return new RecipeFormInputs
        {
            Title = title,
            Summary = null,
            PreparationTimeInMinutes = null,
            CookingTimeInMinutes = null,
            BakingTimeInMinutes = null,
            Servings = null,
            Ingredients = ingredients,
            Directions = directions,
            Images = []
        };
    }
}
