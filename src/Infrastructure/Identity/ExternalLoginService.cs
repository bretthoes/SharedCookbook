using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Infrastructure.Identity;

public class ExternalLoginService(
    UserManager<ApplicationUser> userManager,
    IOptions<GoogleAuthOptions> googleOptions,
    IOptions<AppleAuthOptions> appleOptions,
    IOptions<FacebookAuthOptions> facebookOptions,
    IHttpClientFactory httpClientFactory,
    ILogger<ExternalLoginService> logger)
    : IExternalLoginService
{
    private const string GoogleLoginProvider = "Google";
    private const string AppleLoginProvider = "Apple";
    private const string FacebookLoginProvider = "Facebook";
    private const string AppleIssuer = "https://appleid.apple.com";

    private readonly string _googleClientId = googleOptions.Value.ClientId
        ?? throw new InvalidOperationException("Authentication:Google:ClientId must be configured.");

    private readonly string _appleBundleId = appleOptions.Value.BundleId
        ?? throw new InvalidOperationException("Authentication:Apple:BundleId must be configured.");

    private readonly string _facebookAppId = facebookOptions.Value.AppId
        ?? throw new InvalidOperationException("Authentication:Facebook:AppId must be configured.");

    private readonly string _facebookAppSecret = facebookOptions.Value.AppSecret
        ?? throw new InvalidOperationException("Authentication:Facebook:AppSecret must be configured.");

    private static readonly ConfigurationManager<OpenIdConnectConfiguration> AppleConfigManager = new(
        "https://appleid.apple.com/.well-known/openid-configuration",
        new OpenIdConnectConfigurationRetriever(),
        new HttpDocumentRetriever());

    public async Task<Result<string>> LoginWithGoogleAsync(string idToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(idToken))
        {
            logger.LogWarning("LoginWithGoogle: ID token is required but was empty");
            return Result<string>.Failure(["ID token is required."]);
        }

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [_googleClientId],
                });
        }
        catch (InvalidJwtException ex)
        {
            logger.LogWarning(ex, message: "LoginWithGoogle: Invalid or expired token. Backend ClientId {ClientId}.", _googleClientId);
            return Result<string>.Failure(["Invalid or expired Google token."]);
        }

        string? email = payload.Email;
        string? subject = payload.Subject;

        if (string.IsNullOrEmpty(email))
        {
            logger.LogWarning("LoginWithGoogle: Token validated but did not contain email");
            return Result<string>.Failure(["Google token did not contain email."]);
        }

        return await FindOrCreateUserAsync(GoogleLoginProvider, subject, email);
    }

    public async Task<Result<string>> LoginWithAppleAsync(string identityToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(identityToken))
        {
            logger.LogWarning("LoginWithApple: Identity token is required but was empty");
            return Result<string>.Failure(["Identity token is required."]);
        }

        ClaimsPrincipal principal;
        try
        {
            var config = await AppleConfigManager.GetConfigurationAsync(ct);
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = AppleIssuer,
                ValidAudience = _appleBundleId,
                IssuerSigningKeys = config.SigningKeys,
            };

            var handler = new JwtSecurityTokenHandler();
            principal = handler.ValidateToken(identityToken, validationParameters, out _);
        }
        catch (SecurityTokenException ex)
        {
            logger.LogWarning(ex, "LoginWithApple: Invalid or expired token. BundleId {BundleId}.", _appleBundleId);
            return Result<string>.Failure(["Invalid or expired Apple token."]);
        }

        string? subject = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        string? email = principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(email))
        {
            logger.LogWarning("LoginWithApple: Token validated but did not contain email");
            return Result<string>.Failure(["Apple token did not contain email."]);
        }

        if (string.IsNullOrEmpty(subject))
        {
            logger.LogWarning("LoginWithApple: Token validated but did not contain subject");
            return Result<string>.Failure(["Apple token did not contain subject."]);
        }

        return await FindOrCreateUserAsync(AppleLoginProvider, subject, email);
    }

    public async Task<Result<string>> LoginWithFacebookAsync(string accessToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            logger.LogWarning("LoginWithFacebook: Access token is required but was empty");
            return Result<string>.Failure(["Access token is required."]);
        }

        var httpClient = httpClientFactory.CreateClient("Facebook");

        var appToken = $"{_facebookAppId}|{_facebookAppSecret}";
        var debugUrl = $"https://graph.facebook.com/debug_token?input_token={Uri.EscapeDataString(accessToken)}&access_token={Uri.EscapeDataString(appToken)}";

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

        if (!isValid || appId != _facebookAppId)
        {
            logger.LogWarning("LoginWithFacebook: Token is invalid or belongs to a different app. app_id={AppId}", appId);
            return Result<string>.Failure(["Invalid or expired Facebook token."]);
        }

        var profileUrl = $"https://graph.facebook.com/me?fields=id,email&access_token={Uri.EscapeDataString(accessToken)}";

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

        return await FindOrCreateUserAsync(FacebookLoginProvider, subject, email);
    }

    private async Task<Result<string>> FindOrCreateUserAsync(string loginProvider, string subject, string email)
    {
        var user = await userManager.FindByLoginAsync(loginProvider, providerKey: subject);
        if (user != null)
            return Result<string>.Success(user.Id);

        user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var addLoginResult = await userManager.AddLoginAsync(
                user,
                new UserLoginInfo(loginProvider, providerKey: subject, loginProvider));
            return !addLoginResult.Succeeded
                ? Result<string>.Failure(addLoginResult.Errors.Select(error => error.Description).ToArray())
                : Result<string>.Success(user.Id);
        }

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
        };

        string password = GenerateSecureRandomPassword();
        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
            return Result<string>.Failure(createResult.Errors.Select(error => error.Description).ToArray());

        var linkResult = await userManager.AddLoginAsync(
            user,
            new UserLoginInfo(loginProvider, providerKey: subject, loginProvider));
        if (!linkResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return Result<string>.Failure(linkResult.Errors.Select(error => error.Description).ToArray());
        }

        return Result<string>.Success(user.Id);
    }

    private static string GenerateSecureRandomPassword()
    {
        const int length = 32;
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
        var bytes = new byte[length * 4];
        RandomNumberGenerator.Fill(bytes);
        var result = new char[length];
        for (var i = 0; i < length; i++)
            result[i] = validChars[(int)(BitConverter.ToUInt32(bytes, startIndex: i * 4) % validChars.Length)];
        return new string(result);
    }
}
