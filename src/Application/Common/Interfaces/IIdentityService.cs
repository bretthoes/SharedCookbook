using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<(string? Email, string? DisplayName)?> FindByEmailAsync(string email);
        Task<(string? Email, string? DisplayName)?> FindByIdAsync(string id);

        Task<string?> GetIdByEmailAsync(string email);
        Task<string?> GetUserNameAsync(string userId);
        Task<string?> GetEmailAsync(string userId);
        Task<string?> GetDisplayNameAsync(string userId);

        Task<bool> IsInRoleAsync(string userId, string role);
        Task<bool> AuthorizeAsync(string userId, string policyName);

        Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);
        Task<Result> DeleteUserAsync(string userId);
        Task<Result> UpdateUserAsync(string userId, string displayName);
    }
}
