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

// TODO class needs refactoring
public class RecipeUrlParser(
    IOptions<RecipeUrlParserOptions> options,
    IImageUploadService imageUploadService,
    ILogger<RecipeUrlParser> logger)
    : IRecipeUrlParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly RecipeUrlParserOptions _options = options.Value;

    public async Task<CreateRecipeDto> Parse(string url, CancellationToken cancellationToken)
    {
        // Construct the full Spoonacular API URL with query parameters
        var apiUrl = $"{_options.BaseUrl}/recipes/extract";
        
        var request = GetRequest(url);
        
        // Create the RestClient
        using var client = new RestClient(apiUrl);

        // Execute the request
        var response = await client.ExecuteAsync(request, cancellationToken);

        if (response.IsSuccessful)
        {
            try
            {
                var apiResponse = JsonSerializer.Deserialize<RecipeApiResponse>(response.Content ?? "", JsonOptions);

                if (apiResponse != null)
                {
                    return await MapToCreateRecipeDto(apiResponse, cookbookId: 0);
                }

                throw new Exception("Failed to deserialize the recipe data.");
            }
            catch (JsonException ex)
            {
                logger.LogError("Error deserializing response content: {Error}", ex.Message);
                throw new Exception("Error parsing recipe data from API response.", ex);
            }
        }

        logger.LogError(
            "Failed to parse recipe from URL: {StatusCode} {Content} {Exception}",
            response.StatusCode,
            response.Content,
            response.ErrorException?.Message);

        throw new Exception("Failed to fetch or parse the recipe from the URL.");
    }
    
    private RestRequest GetRequest(string url) =>
        new RestRequest()
            .AddParameter(name: "url", url)
            .AddParameter(name: "forceExtraction", "true")
            .AddParameter(name: "analyze", "false")
            .AddParameter(name: "includeNutrition", "false")
            .AddParameter(name: "includeTaste", "false")
            .AddParameter(name: "apiKey", _options.ApiKey);

    private async Task<CreateRecipeDto> MapToCreateRecipeDto(RecipeApiResponse apiResponse, int cookbookId)
    {
        // Split instructions into individual steps based on double newline characters
        string summary = apiResponse.Summary?.RemoveHtml() ?? "";
        string summaryDecoded = WebUtility.HtmlDecode(summary);
        string instructionsDecoded = WebUtility.HtmlDecode(apiResponse.Instructions);
        string[] steps = instructionsDecoded.Split(["\n\n"], StringSplitOptions.RemoveEmptyEntries);
        foreach (string step in steps) step.RemoveHtml();
        
        var hasImage = apiResponse.Image?.IsValidUrl() ?? false;
        var image = "";
        if (apiResponse.Image != null && hasImage)
            image = await imageUploadService.UploadImageFromUrl(apiResponse.Image);

        var createRecipeDto = new CreateRecipeDto
        {
            Title = apiResponse.Title.Length > 255 ? apiResponse.Title[..255] : apiResponse.Title,
            Images = string.IsNullOrWhiteSpace(image)
                ? []
                : [new CreateRecipeImageDto { Name= image, Ordinal = 1 }],
            CookbookId = cookbookId,
            Summary = summaryDecoded.Length > 2048 ? summaryDecoded[..2048] : summaryDecoded,
            Servings = apiResponse.Servings,
            PreparationTimeInMinutes = apiResponse.PreparationMinutes,
            CookingTimeInMinutes = apiResponse.CookingMinutes,
            BakingTimeInMinutes = null,
            Ingredients = apiResponse.ExtendedIngredients.Select((ingredient, index) =>
                new CreateRecipeIngredientDto
                {
                    Name = ingredient.Original.Length > 255 ? ingredient.Original[..255] : ingredient.Original,
                    Optional = false,
                    Ordinal = index + 1
                }).ToList(),
            Directions = steps.Select((stepString, index) =>
                new CreateRecipeDirectionDto
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

