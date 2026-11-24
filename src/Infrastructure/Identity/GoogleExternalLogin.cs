using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Users.Commands.ExternalLogin;

namespace SharedCookbook.Infrastructure.Identity;

public sealed class GoogleExternalLogin(
    ExternalAccountLinker accountLinker,
    IOptions<GoogleAuthOptions> options,
    ILogger<GoogleExternalLogin> logger) : IExternalLoginProvider
{
    private const string LoginProvider = "Google";

    private readonly string _clientId = options.Value.ClientId
        ?? throw new InvalidOperationException("Authentication:Google:ClientId must be configured.");

    public ExternalLoginProvider Provider => ExternalLoginProvider.Google;

    public async Task<Result<string>> LoginAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            logger.LogWarning("LoginWithGoogle: ID token is required but was empty");
            return Result<string>.Failure(["ID token is required."]);
        }

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(
                token,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [_clientId],
                });
        }
        catch (InvalidJwtException ex)
        {
            logger.LogWarning(ex, "LoginWithGoogle: Invalid or expired token. Backend ClientId {ClientId}.", _clientId);
            return Result<string>.Failure(["Invalid or expired Google token."]);
        }

        string? email = payload.Email;
        string? subject = payload.Subject;

        if (!string.IsNullOrWhiteSpace(email))
            return await accountLinker.FindOrCreateUserAsync(LoginProvider, subject, email, payload.EmailVerified);

        logger.LogWarning("LoginWithGoogle: Token validated but did not contain email");
        return Result<string>.Failure(["Google token did not contain email."]);
    }
}
