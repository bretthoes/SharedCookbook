using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SharedCookbook.Application.FunctionalTests.RateLimiting;

internal sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    internal const string SchemeName = "Test";
    internal const string UserIdHeaderName = "X-Test-User-Id";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(UserIdHeaderName, out var userIdValues)
            || string.IsNullOrWhiteSpace(userIdValues.FirstOrDefault()))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userIdValues.First()!) };
        var identity = new ClaimsIdentity(claims, authenticationType: SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

internal static class RecipeParsingRateLimitTestClient
{
    internal const string ParseRecipeVoicePath = "/api/Recipes/parse-recipe-voice";

    internal static Task<HttpResponseMessage> ParseRecipeFromVoiceAsync(
        HttpClient client,
        string userId,
        string transcript = "one cup flour")
    {
        var request = new HttpRequestMessage(HttpMethod.Post, ParseRecipeVoicePath);
        request.Headers.Add(TestAuthHandler.UserIdHeaderName, userId);
        request.Headers.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemeName);
        request.Content = JsonContent.Create(new { transcript });

        return client.SendAsync(request);
    }
}

internal static class ImageUploadRateLimitTestClient
{
    internal const string UploadImagesPath = "/api/Images";
    private static readonly byte[] MinimalJpegBytes = [0xFF, 0xD8, 0xFF, 0xD9];

    internal static Task<HttpResponseMessage> UploadImageAsync(HttpClient client, string userId)
    {
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(MinimalJpegBytes), "files", "test.jpg");

        var request = new HttpRequestMessage(HttpMethod.Post, UploadImagesPath)
        {
            Content = content,
        };
        request.Headers.Add(TestAuthHandler.UserIdHeaderName, userId);
        request.Headers.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemeName);

        return client.SendAsync(request);
    }
}
