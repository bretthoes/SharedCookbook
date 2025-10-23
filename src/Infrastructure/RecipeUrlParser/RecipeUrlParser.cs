using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using System.Text.Json;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.RecipeUrlParser.Models;

namespace SharedCookbook.Infrastructure.RecipeUrlParser;

// TODO class needs refactoring; also throw more specific exceptions
public class RecipeUrlParser(
    IOptions<RecipeUrlParserOptions> options,
    IImageUploader imageUploader,
    ILogger<RecipeUrlParser> logger) : IRecipeUrlParser
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<CreateRecipeDto> Parse(string url, CancellationToken cancellationToken)
    {
        // Construct the full Spoonacular API URL with query parameters
        var apiUrl = $"{options.Value.BaseUrl}/recipes/extract";

        var request = GetRequest(url);

        // Create the RestClient
        using var client = new RestClient(apiUrl);

        // Execute the request
        var response = await client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful)
        {
            logger.LogError(
                "Failed to parse recipe from URL: {StatusCode} {Content} {Exception}",
                response.StatusCode,
                response.Content,
                response.ErrorException?.Message);

            throw new Exception("Failed to fetch or parse the recipe from the URL.");
        }

        try
        {
            var apiResponse = JsonSerializer.Deserialize<RecipeApiResponse>(response.Content ?? "", JsonOptions);
            if (apiResponse is null)
                throw new Exception("Failed to deserialize the recipe data.");

            return await MapToCreateRecipeDto(apiResponse.ApplyDefaults());
        }
        catch (JsonException ex)
        {
            logger.LogError("Error deserializing response content: {Error}", ex.Message);
            throw new Exception("Error parsing recipe data from API response.", ex);
        }
    }

    private RestRequest GetRequest(string url) =>
        new RestRequest()
            .AddParameter(name: "url", url)
            .AddParameter(name: "forceExtraction", "true")
            .AddParameter(name: "analyze", "false")
            .AddParameter(name: "includeNutrition", "false")
            .AddParameter(name: "includeTaste", "false")
            .AddParameter(name: "apiKey", options.Value.ApiKey);

    private async Task<CreateRecipeDto> MapToCreateRecipeDto(RecipeApiResponse apiResponse)
    {
        // Split instructions into individual steps based on double newline characters
        string summary = apiResponse.Summary?.RemoveHtml() ?? "";
        string summaryDecoded = WebUtility.HtmlDecode(summary);
        string instructionsDecoded = WebUtility.HtmlDecode(apiResponse.Instructions) ?? "";

        string[] steps = instructionsDecoded
            .Split(["\n\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(stepString => stepString.RemoveHtml().Trim())
            .Where(stepString => stepString.Length > 0)
            .ToArray();

        var image = "";
        if (apiResponse.HasImage())
            image = await imageUploader.UploadImageFromUrl(apiResponse.Image!);

        string title = apiResponse.Title?.Truncate(Recipe.Constraints.TitleMaxLength) ?? "";

        var createRecipeDto = new CreateRecipeDto
        {
            Title = title,
            Images = string.IsNullOrWhiteSpace(image)
                ? []
                : [new RecipeImageDto { Name = image, Ordinal = 1 }],
            CookbookId = 0,
            Summary = summaryDecoded.Truncate(Recipe.Constraints.SummaryMaxLength),
            Servings = apiResponse.Servings,
            PreparationTimeInMinutes = apiResponse.PreparationMinutes,
            CookingTimeInMinutes = apiResponse.CookingMinutes,
            BakingTimeInMinutes = null,
            Ingredients = apiResponse.ExtendedIngredients?.Select((ingredient, index) =>
                new RecipeIngredientDto
                {
                    Name = ingredient.Original.Truncate(RecipeIngredient.Constraints.NameMaxLength),
                    Optional = false,
                    Ordinal = index + 1
                }).ToList() ?? [],
            Directions = steps.Select((stepString, index) =>
                new RecipeDirectionDto
                {
                    Text = stepString.Length > RecipeDirection.Constraints.TextMaxLength
                        ? stepString[..RecipeDirection.Constraints.TextMaxLength]
                        : stepString,
                    Ordinal = index + 1
                }).ToList()
        };

        return createRecipeDto;
    }
}
