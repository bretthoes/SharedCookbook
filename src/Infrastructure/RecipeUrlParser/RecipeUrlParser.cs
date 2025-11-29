using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Interfaces;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.RecipeUrlParser.Models;

namespace SharedCookbook.Infrastructure.RecipeUrlParser;

// TODO class needs refactoring; also throw more specific exceptions; rename class indicating the impl (spoonocular)
public class RecipeUrlParser(
    IOptions<RecipeUrlParserOptions> options,
    IImageUploader imageUploader,
    IHttpClientFactory clientFactory,
    ILogger<RecipeUrlParser> logger) : IRecipeUrlParser
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<CreateRecipeDto> Parse(string url, CancellationToken cancellationToken)
    {
        // Construct the full Spoonacular API URL with query parameters
        string apiUrl = $"{options.Value.BaseUrl}/recipes/extract";

        var queryParams = new Dictionary<string, string?>
        {
            ["url"] = url,
            ["forceExtraction"] = "true",
            ["analyze"] = "false",
            ["includeNutrition"] = "false",
            ["includeTaste"] = "false",
            ["apiKey"] = options.Value.ApiKey
        };
        string requestUri = QueryHelpers.AddQueryString(apiUrl, queryParams!);

        var http = clientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.ParseAdd("application/json");

        using var response = await http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Failed to parse recipe from URL: {StatusCode} {ReasonPhrase}",
                response.StatusCode,
                response.ReasonPhrase);

            throw new HttpRequestException("Failed to fetch or parse the recipe from the URL.");
        }

        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        var apiResponse = JsonSerializer.Deserialize<RecipeApiResponse>(content, JsonOptions)
            ?? throw new JsonException("Received null payload.");

        return await MapToCreateRecipeDto(apiResponse.ApplyDefaults());
    }

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

        return new CreateRecipeDto
        {
            Title = title,
            Images = string.IsNullOrWhiteSpace(image) ? [] : [new RecipeImageDto { Name = image, Ordinal = 1 }],
            CookbookId = 0,
            Summary = summaryDecoded.Truncate(Recipe.Constraints.SummaryMaxLength),
            Servings = apiResponse.Servings,
            PreparationTimeInMinutes = apiResponse.PreparationMinutes,
            CookingTimeInMinutes = apiResponse.CookingMinutes,
            BakingTimeInMinutes = null,
            Ingredients = apiResponse.ExtendedIngredients.ToDtos(),
            Directions = steps.Select((stepString, index) =>
                new RecipeDirectionDto
                {
                    Text = stepString.Length > RecipeDirection.Constraints.TextMaxLength
                        ? stepString[..RecipeDirection.Constraints.TextMaxLength]
                        : stepString,
                    Ordinal = index + 1
                }).ToList()
        };
    }
}
