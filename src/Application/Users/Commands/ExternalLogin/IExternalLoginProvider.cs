using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Users.Commands.ExternalLogin;

// TODO: Returns Result<T>, while the rest of the application throws
// NotFoundException, ForbiddenAccessException, and ValidationException.
// See the same note on IIdentityService. These two error styles should be unified.
public interface IExternalLoginProvider
{
    ExternalLoginProvider Provider { get; }

    Task<Result<string>> LoginAsync(string token, CancellationToken ct = default);
}
