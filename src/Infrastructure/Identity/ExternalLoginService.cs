using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
    ILogger<ExternalLoginService> logger)
    : IExternalLoginService
{
    private const string GoogleLoginProvider = "Google";
    private const string AppleLoginProvider = "Apple";
    private const string AppleIssuer = "https://appleid.apple.com";

    private readonly string _googleClientId = googleOptions.Value.ClientId
        ?? throw new InvalidOperationException("Authentication:Google:ClientId must be configured.");

    private readonly string _appleBundleId = appleOptions.Value.BundleId
        ?? throw new InvalidOperationException("Authentication:Apple:BundleId must be configured.");

    private static readonly ConfigurationManager<OpenIdConnectConfiguration> AppleConfigManager = new(
        "https://appleid.apple.com/.well-known/openid-configuration",
        new OpenIdConnectConfigurationRetriever(),
        new HttpDocumentRetriever());

    public async Task<Result<string>> LoginWithGoogleAsync(string idToken, CancellationToken cancellationToken)
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

    public async Task<Result<string>> LoginWithAppleAsync(string identityToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(identityToken))
        {
            logger.LogWarning("LoginWithApple: Identity token is required but was empty");
            return Result<string>.Failure(["Identity token is required."]);
        }

        ClaimsPrincipal principal;
        try
        {
            var config = await AppleConfigManager.GetConfigurationAsync(cancellationToken);
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
