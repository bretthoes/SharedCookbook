using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using System.Text.Json;

namespace SharedCookbook.Infrastructure.RecipeUrlParser
{
    public class RecipeUrlParser(
        IOptions<RecipeUrlParserOptions> options,
        ILogger<RecipeUrlParser> logger)
        : IRecipeUrlParser
    {
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
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var apiResponse = JsonSerializer.Deserialize<RecipeApiResponse>(response.Content ?? "", options);


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
            // TODO get image, prep time from online recipe as well
            var createRecipeDto = new CreateRecipeDto
            {
                Title = apiResponse.Title,
                CookbookId = cookbookId,
                Summary = apiResponse.Summary,
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
                Directions = apiResponse.AnalyzedInstructions.Select((instruction, _) =>
                    new CreateRecipeDirectionDto
                    {
                        Text = instruction.Step,
                        Ordinal = instruction.Number
                    }).ToList()
            };

            return createRecipeDto;
        }
    }

    // These only exist to deserialize based on matching property names from
    // Spoonacular API; may be better to move these to specific dto folder
    public class RecipeApiResponse
    {
        public string Title { get; set; } = "";
        public string? Summary { get; set; }
        public string? Image { get; set; }
        public int? Servings { get; set; }
        public List<Ingredient> ExtendedIngredients { get; set; } = [];
        public List<InstructionStep> AnalyzedInstructions { get; set; } = [];
    }

    public class Ingredient
    {
        public string Name { get; set; } = "";
        public bool Optional { get; set; }
    }

    public class InstructionStep
    {
        public string Step { get; set; } = "";
        public int Number { get; set; }
    }
}
