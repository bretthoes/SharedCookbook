using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Interfaces;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using SharedCookbook.Infrastructure.RecipeUrlParser.Models;

namespace SharedCookbook.Infrastructure.RecipeUrlParser;

public sealed class SpoonacularApiParser(
    IOptions<RecipeUrlParserOptions> options,
    IImageUploader imageUploader,
    IHttpClientFactory clientFactory,
    ILogger<SpoonacularApiParser> logger) : IRecipeUrlParser
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly string _apiUrl = $"{options.Value.BaseUrl}/recipes/extract";
    private readonly string _true = true.ToString().ToLowerInvariant();
    private readonly string _false = false.ToString().ToLowerInvariant();

    public async Task<CreateRecipeDto> Parse(
        string url,
        bool extractFromVideo = false,
        CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["url"] = url,
            ["forceExtraction"] = _true,
            ["analyze"] = _false,
            ["includeNutrition"] = _false,
            ["includeTaste"] = _false,
            ["extractFromVideo"] = extractFromVideo ? _true : _false,
            ["apiKey"] = options.Value.ApiKey
        };
        string requestUri = QueryHelpers.AddQueryString(_apiUrl, queryParams);

        using var http = clientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.ParseAdd("application/json");

        using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Failed to parse recipe from URL: {StatusCode} {ReasonPhrase}",
                response.StatusCode,
                response.ReasonPhrase);

            throw new HttpRequestException("Failed to fetch or parse the recipe from the URL.");
        }

        var apiResponse = JsonSerializer.Deserialize<RecipeApiResponse>(
            await response.Content.ReadAsStringAsync(ct), 
            JsonOptions) 
            ?? throw new JsonException("Received null payload.");


        string? uploadedKey = null;
        if (!apiResponse.HasImage()) return apiResponse.ToDto(uploadedKey);
        try { uploadedKey = await imageUploader.UploadImageFromUrl(apiResponse.Image!, ct); }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        // swallow exception if image upload fails; don't let a bad or
        // missing image stop a user from importing a recipe from a URL
        catch (Exception ex) 
        {
            logger.LogWarning(
                ex,
                message: "Spoonacular image upload failed, continuing without image: {ImageUrl}",
                apiResponse.Image);
        }

        return apiResponse.ToDto(uploadedKey);
    }
}
