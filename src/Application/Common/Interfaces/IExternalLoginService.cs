using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Common.Interfaces;

// TODO: Returns Result<T>, while the rest of the application throws
// NotFoundException, ForbiddenAccessException, and ValidationException.
// See the same note on IIdentityService. These two error styles should be unified.
public interface IExternalLoginService
{
    /// <summary>
    /// Validates the Google ID token, finds or creates the user, and returns the user ID on success.
    /// </summary>
    Task<Result<string>> LoginWithGoogleAsync(string idToken, CancellationToken ct = default);

    /// <summary>
    /// Validates the Apple identity token, finds or creates the user, and returns the user ID on success.
    /// </summary>
    Task<Result<string>> LoginWithAppleAsync(string identityToken, CancellationToken ct = default);

    /// <summary>
    /// Validates the Facebook access token, finds or creates the user, and returns the user ID on success.
    /// </summary>
    Task<Result<string>> LoginWithFacebookAsync(string accessToken, CancellationToken ct = default);
}
