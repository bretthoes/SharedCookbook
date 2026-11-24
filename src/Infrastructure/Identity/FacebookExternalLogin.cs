using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Users.Commands.ExternalLogin;

namespace SharedCookbook.Infrastructure.Identity;

public sealed class FacebookExternalLogin(
    ExternalAccountLinker accountLinker,
    IOptions<FacebookAuthOptions> options,
    IHttpClientFactory httpClientFactory,
    ILogger<FacebookExternalLogin> logger) : IExternalLoginProvider
{
    private const string LoginProvider = "Facebook";

    private readonly string _appId = options.Value.AppId
        ?? throw new InvalidOperationException("Authentication:Facebook:AppId must be configured.");

    private readonly string _appSecret = options.Value.AppSecret
        ?? throw new InvalidOperationException("Authentication:Facebook:AppSecret must be configured.");

    public ExternalLoginProvider Provider => ExternalLoginProvider.Facebook;

    public async Task<Result<string>> LoginAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            logger.LogWarning("LoginWithFacebook: Access token is required but was empty");
            return Result<string>.Failure(["Access token is required."]);
        }

        var httpClient = httpClientFactory.CreateClient("Facebook");

        var appToken = $"{_appId}|{_appSecret}";
        var debugUrl =
            $"https://graph.facebook.com/debug_token?input_token={Uri.EscapeDataString(token)}&access_token={Uri.EscapeDataString(appToken)}";

        JsonDocument debugDoc;
        try
        {
            var debugResponse = await httpClient.GetAsync(debugUrl, ct);
            debugResponse.EnsureSuccessStatusCode();
            var debugJson = await debugResponse.Content.ReadAsStringAsync(ct);
            debugDoc = JsonDocument.Parse(debugJson);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "LoginWithFacebook: Failed to reach Facebook debug_token endpoint");
            return Result<string>.Failure(["Could not verify Facebook token."]);
        }

        var data = debugDoc.RootElement.GetProperty("data");
        var isValid = data.TryGetProperty("is_valid", out var isValidProp) && isValidProp.GetBoolean();
        var appId = data.TryGetProperty("app_id", out var appIdProp) ? appIdProp.GetString() : null;

        if (!isValid || appId != _appId)
        {
            logger.LogWarning("LoginWithFacebook: Token is invalid or belongs to a different app. app_id={AppId}", appId);
            return Result<string>.Failure(["Invalid or expired Facebook token."]);
        }

        var profileUrl = $"https://graph.facebook.com/me?fields=id,email&access_token={Uri.EscapeDataString(token)}";

        JsonDocument profileDoc;
        try
        {
            var profileResponse = await httpClient.GetAsync(profileUrl, ct);
            profileResponse.EnsureSuccessStatusCode();
            var profileJson = await profileResponse.Content.ReadAsStringAsync(ct);
            profileDoc = JsonDocument.Parse(profileJson);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "LoginWithFacebook: Failed to fetch user profile from Graph API");
            return Result<string>.Failure(["Could not retrieve Facebook profile."]);
        }

        var root = profileDoc.RootElement;
        var subject = root.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
        var email = root.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;

        if (string.IsNullOrEmpty(email))
        {
            logger.LogWarning("LoginWithFacebook: Profile did not contain email. User may not have granted email permission.");
            return Result<string>.Failure(["Facebook account did not provide an email address."]);
        }

        if (string.IsNullOrEmpty(subject))
        {
            logger.LogWarning("LoginWithFacebook: Profile did not contain id");
            return Result<string>.Failure(["Facebook profile did not contain a user ID."]);
        }

        // The Graph API does not say whether the email was verified.
        return await accountLinker.FindOrCreateUserAsync(LoginProvider, subject, email, isEmailVerified: false);
    }
}
