using System.Net.Http.Headers;
using System.Net.Http.Json;
using SharedCookbook.Application.Common;
using SharedCookbook.Application.FunctionalTests.Infrastructure;

namespace SharedCookbook.Application.FunctionalTests.RateLimiting;

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

    internal static Task<HttpResponseMessage> UploadOversizedImageAsync(HttpClient client, string userId)
    {
        var oversizedBytes = new byte[ImageUtilities.MaxFileSizeBytes + 1];
        Array.Copy(MinimalJpegBytes, oversizedBytes, MinimalJpegBytes.Length);

        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(oversizedBytes), "files", "test.jpg");

        var request = new HttpRequestMessage(HttpMethod.Post, UploadImagesPath)
        {
            Content = content,
        };
        request.Headers.Add(TestAuthHandler.UserIdHeaderName, userId);
        request.Headers.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemeName);

        return client.SendAsync(request);
    }
}
