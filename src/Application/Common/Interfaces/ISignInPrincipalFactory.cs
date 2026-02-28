using System.Security.Claims;

namespace SharedCookbook.Application.Common.Interfaces;

public interface ISignInPrincipalFactory
{
    Task<ClaimsPrincipal?> CreatePrincipalForUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
