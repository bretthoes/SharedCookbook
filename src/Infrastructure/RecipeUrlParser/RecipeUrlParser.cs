using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using System.Text.Json;

namespace SharedCookbook.Infrastructure.RecipeUrlParser;

public class RecipeUrlParser(
    IOptions<RecipeUrlParserOptions> options,
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

        // Create the RestClient
        using var client = new RestClient(apiUrl);

        // Create the RestRequest and add the query parameters
        var request = new RestRequest();

        // Add query parameters for the request
        request.AddParameter(name: "url", url);
        request.AddParameter(name: "forceExtraction", "true");
        request.AddParameter(name: "analyze", "false");
        request.AddParameter(name: "includeNutrition", "false");
        request.AddParameter(name: "includeTaste", "false");
        request.AddParameter(name: "apiKey", _options.ApiKey);

        // Execute the request
        var response = await client.ExecuteAsync(request, cancellationToken);

        if (response.IsSuccessful)
        {
            try
            {
                var apiResponse = JsonSerializer.Deserialize<RecipeApiResponse>(response.Content ?? "", JsonOptions);

                if (apiResponse != null)
                {
                    return MapToCreateRecipeDto(apiResponse, 0);
                }
                else
                {
                    throw new Exception("Failed to deserialize the recipe data.");
                }
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

    private static CreateRecipeDto MapToCreateRecipeDto(RecipeApiResponse apiResponse, int cookbookId)
    {
        // Split instructions into individual steps based on double newline characters
        var steps = apiResponse.Instructions.Split(["\n\n"], StringSplitOptions.RemoveEmptyEntries);

        var createRecipeDto = new CreateRecipeDto
        {
            Title = apiResponse.Title.Length > 255 ? apiResponse.Title[..255] : apiResponse.Title,
            CookbookId = cookbookId,
            // TODO need to handle html in here being viewed as plain text. Also should increase length in db
            Summary = apiResponse.Summary?.Length > 255 ? apiResponse.Summary[..255] : apiResponse.Summary,
            Servings = apiResponse.Servings,
            PreparationTimeInMinutes = null,
            CookingTimeInMinutes = null,
            BakingTimeInMinutes = null,
            Ingredients = apiResponse.ExtendedIngredients.Select((ingredient, index) =>
                new CreateRecipeIngredientDto
                {
                    Name = ingredient.Name,
                    Optional = ingredient.Optional,
                    Ordinal = index + 1
                }).ToList(),
            Directions = steps.Select((step, index) =>
                new CreateRecipeDirectionDto
                {
                    Text = step.Length > 255 ? step[..255] : step,
                    Ordinal = index + 1
                }).ToList()
        };

        return createRecipeDto;
    }
}

// These only exist to deserialize based on matching property names from
// Spoonacular API; may be better to move these to specific dto folder
public class RecipeApiResponse
{
    public string Title { get; init; } = "";
    public string? Summary { get; init; }
    public string? Image { get; init; }
    public int? Servings { get; init; }
    public List<Ingredient> ExtendedIngredients { get; init; } = [];
    public string Instructions { get; init; } = "";
}

public class Ingredient
{
    public string Name { get; set; } = "";
    public bool Optional { get; set; }
}

