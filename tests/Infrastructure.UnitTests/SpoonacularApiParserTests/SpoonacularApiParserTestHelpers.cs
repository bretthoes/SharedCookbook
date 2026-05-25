using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Infrastructure.RecipeUrlParser;

namespace SharedCookbook.Infrastructure.UnitTests.SpoonacularApiParserTests;

internal sealed class StubHttpClientFactory(HttpMessageHandler handler) : IHttpClientFactory
{
    public HttpClient CreateClient(string name) => new(handler, disposeHandler: false);
}

internal sealed class StubHttpMessageHandler(
    Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
    : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken ct = default) =>
        handler(request, ct);
}

internal static class SpoonacularApiParserTestHelpers
{
    internal static SpoonacularApiParser BuildSut(
        Mock<IImageUploader> imageUploader,
        HttpResponseMessage responseMessage)
    {
        var options = Options.Create(new RecipeUrlParserOptions
        {
            BaseUrl = "https://api.spoonacular.test",
            ApiKey = "test-api-key"
        });

        var handler = new StubHttpMessageHandler((_, _) => Task.FromResult(responseMessage));
        var clientFactory = new StubHttpClientFactory(handler);
        var logger = new Mock<ILogger<SpoonacularApiParser>>();

        return new SpoonacularApiParser(
            options,
            imageUploader.Object,
            clientFactory,
            logger.Object);
    }

    internal static HttpResponseMessage BuildSuccessResponse(string? imageUrl)
    {
        const string prefix = """
                              {
                                "title": "Imported Recipe",
                                "summary": "A parsed recipe",
                                "servings": 2,
                                "extendedIngredients": [
                                  { "original": "1 egg" }
                                ],
                                "instructions": "1. Mix ingredients",
                                "preparationMinutes": 5,
                                "cookingMinutes": 10,
                              """;
        string imageProperty = imageUrl is null
            ? "\"image\": null"
            : $"\"image\": \"{imageUrl}\"";
        string body = prefix + imageProperty + "\n}";

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
    }
}
