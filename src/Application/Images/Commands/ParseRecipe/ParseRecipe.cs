﻿using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;

namespace SharedCookbook.Application.Images.Commands.ParseRecipe;

public record ParseRecipeCommand(IFormFile File) : IRequest<CreateRecipeDto>;

public class ParseRecipeCommandHandler(IOcrService ocrService)
    : IRequestHandler<ParseRecipeCommand, CreateRecipeDto>
{
    public async Task<CreateRecipeDto> Handle(ParseRecipeCommand request, CancellationToken cancellationToken)
    {
        string extractedText = await ocrService.ExtractText(request.File);
        return ParseRecipeFromText(extractedText);
    }

    private static CreateRecipeDto ParseRecipeFromText(string text)
    {
        var lines = GetLines(text);
        string title = GetTitle(lines);
        var ingredients = ParseIngredients(text);
        var directions = ParseDirections(text);

        return CreateRecipeDto(title, ingredients, directions);
    }

    private static string[] GetLines(string text) => 
        text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

    private static string GetTitle(string[] lines) => 
        lines.FirstOrDefault()?.Trim() ?? string.Empty;

    private static List<CreateRecipeIngredientDto> ParseIngredients(string text)
    {
        var ingredients = new List<CreateRecipeIngredientDto>();
        var ingredientRegex = new Regex(@"^- (.+?)( \(optional\))?$", RegexOptions.Multiline);

        foreach (Match match in ingredientRegex.Matches(text))
        {
            ingredients.Add(new CreateRecipeIngredientDto
            {
                Name = match.Groups[1].Value,
                Optional = match.Groups[2].Success,
                Ordinal = 0
            });
        }

        return ingredients;
    }

    private static List<CreateRecipeDirectionDto> ParseDirections(string text)
    {
        var directions = new List<CreateRecipeDirectionDto>();
        var directionsRegex = new Regex(@"^(Steps|Directions|Instructions):", 
            RegexOptions.IgnoreCase | RegexOptions.Multiline);
        var directionsMatch = directionsRegex.Match(text);

        if (!directionsMatch.Success) return directions;

        string[] directionLines = text[directionsMatch.Index..]
            .Split('\n')
            .Skip(1) // Skip header
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        return directionLines.Select(step => new CreateRecipeDirectionDto
        {
            Text = step,
            Image = null,
            Ordinal = 0
        }).ToList();
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
}
