using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Users.Commands.ExternalLogin;

namespace SharedCookbook.Infrastructure.Identity;

public sealed class AppleExternalLogin(
    UserManager<ApplicationUser> userManager,
    ExternalAccountLinker accountLinker,
    IOptions<AppleAuthOptions> options,
    ILogger<AppleExternalLogin> logger) : IExternalLoginProvider
{
    private const string LoginProvider = "Apple";
    private const string AppleIssuer = "https://appleid.apple.com";

    private readonly string _bundleId = options.Value.BundleId
        ?? throw new InvalidOperationException("Authentication:Apple:BundleId must be configured.");

    private static readonly ConfigurationManager<OpenIdConnectConfiguration> AppleConfigManager = new(
        "https://appleid.apple.com/.well-known/openid-configuration",
        new OpenIdConnectConfigurationRetriever(),
        new HttpDocumentRetriever());

    public ExternalLoginProvider Provider => ExternalLoginProvider.Apple;

    public async Task<Result<string>> LoginAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token))
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
                ValidAudience = _bundleId,
                IssuerSigningKeys = config.SigningKeys,
            };

            // Keep Apple's short claim names ("sub", "email"). Inbound mapping renames them and
            // has changed defaults across IdentityModel versions.
            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            principal = handler.ValidateToken(token, validationParameters, out _);
        }
        catch (SecurityTokenException ex)
        {
            logger.LogWarning(ex, "LoginWithApple: Invalid or expired token. BundleId {BundleId}.", _bundleId);
            return Result<string>.Failure(["Invalid or expired Apple token."]);
        }

        string? subject = principal.FindFirst("sub")?.Value;
        string? email = principal.FindFirst("email")?.Value;

        if (string.IsNullOrWhiteSpace(subject))
        {
            logger.LogWarning("LoginWithApple: Token validated but did not contain subject");
            return Result<string>.Failure(["Apple token did not contain subject."]);
        }

        // Apple includes email only on the first authorization. Later sign-ins still include
        // the stable subject, which is how we find the account created the first time.
        if (string.IsNullOrWhiteSpace(email))
        {
            var existing = await userManager.FindByLoginAsync(LoginProvider, subject);
            if (existing is not null)
                return Result<string>.Success(existing.Id);

            logger.LogWarning("LoginWithApple: No account for subject and token did not contain email");
            return Result<string>.Failure(["Apple token did not contain email."]);
        }

        // Apple sends email_verified as either a boolean or the string "true".
        bool isEmailVerified = string.Equals(
            principal.FindFirst("email_verified")?.Value,
            "true",
            StringComparison.OrdinalIgnoreCase);

        return await accountLinker.FindOrCreateUserAsync(LoginProvider, subject, email, isEmailVerified);
    }
}
