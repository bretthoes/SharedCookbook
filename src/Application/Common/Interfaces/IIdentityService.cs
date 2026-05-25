using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<(string? Email, string? DisplayName)?> FindByEmailAsync(string email, CancellationToken ct = default);

        Task<(string? Email, string? DisplayName)?> FindByIdAsync(string id, CancellationToken ct = default);

        Task<string?> GetIdByEmailAsync(string email, CancellationToken ct = default);

        Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default);

        Task<string?> GetEmailAsync(string userId, CancellationToken ct = default);

        Task<string?> GetDisplayNameAsync(string userId, CancellationToken ct = default);

        Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default);

        Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken ct = default);

        Task<(Result Result, string UserId)> CreateUserAsync(
            string userName,
            string password,
            CancellationToken ct = default);

        Task<Result> DeleteUserAsync(string userId, CancellationToken ct = default);

        Task<Result> UpdateUserAsync(string userId, string displayName, CancellationToken ct = default);

        Task<SubscriptionTierUpdateResult> SetSubscriptionTierIfChangedAsync(
            string userId,
            string tierName,
            CancellationToken ct = default);
    }
}
