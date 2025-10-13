using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Identity?> FindByEmailAsync(string email);
    
    Task<string?> GetIdByEmailAsync(string email);
    
    Task<Identity?> FindByIdAsync(string id);
    
    Task<string?> GetUserNameAsync(string userId);
    
    Task<string?> GetEmailAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);

    Task<Result> UpdateUserAsync(string userId, string displayName);

    Task<string?> GetDisplayNameAsync(string userId);
}

/// <summary>
/// This record exists as a DTO since ApplicationUser is unknown by the application project, and both primitive
/// fields are often required for mapping purposes when querying for a specific user in the IIdentityService.
/// </summary>
/// <remarks>
/// TODO just convert this to a tuple for the few returns where it's actually used. Obvious enough.
/// </remarks>
public sealed record Identity(string? Email, string? DisplayName);
