using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IExternalLoginService
{
    /// <summary>
    /// Validates the Google ID token, finds or creates the user, and returns the user ID on success.
    /// </summary>
    Task<Result<string>> LoginWithGoogleAsync(string idToken, CancellationToken cancellationToken);

    /// <summary>
    /// Validates the Apple identity token, finds or creates the user, and returns the user ID on success.
    /// </summary>
    Task<Result<string>> LoginWithAppleAsync(string identityToken, CancellationToken cancellationToken);
}
