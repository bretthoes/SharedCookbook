namespace SharedCookbook.Application.Common.Interfaces;

public interface ISignInPrincipalFactory
{
    Task<System.Security.Claims.ClaimsPrincipal?> CreatePrincipalForUserIdAsync(
        string userId,
        CancellationToken ct = default);
}
