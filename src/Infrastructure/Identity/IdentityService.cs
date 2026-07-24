using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Infrastructure.Identity;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService)
    : IIdentityService
{
    public async Task<(string? Email, string? DisplayName)?> FindByEmailAsync(
        string email,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user == null ? null : (user.Email, user.DisplayName);
    }

    public async Task<(string? Email, string? DisplayName)?> FindByIdAsync(
        string id,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(id);
        return user == null ? null : (user.Email, user.DisplayName);
    }

    public async Task<string?> GetIdByEmailAsync(string email, CancellationToken ct = default)
        => (await userManager.FindByEmailAsync(email))?.Id;
    
    public async Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default)
        => (await userManager.FindByIdAsync(userId))?.UserName;
    
    public async Task<string?> GetDisplayNameAsync(string userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);

        return !string.IsNullOrWhiteSpace(user?.DisplayName)
            ? user.DisplayName
            : user?.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(
        string userName,
        string password,
        CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);

        return user != null && await userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(
        string userId,
        string policyName,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await userClaimsPrincipalFactory.CreateAsync(user);

        var result = await authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);

        return user != null
            ? await DeleteUserAsync(user, ct)
            : Result.Success();
    }

    public async Task<Result> UpdateUserAsync(
        string userId,
        string displayName,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        
        if (user is null) return Result.Failure(["User not found."]);

        user.DisplayName = displayName;

        var result = await userManager.UpdateAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<SubscriptionTierUpdateResult> SetSubscriptionTierIfChangedAsync(
        string userId,
        string tierName,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return new SubscriptionTierUpdateResult(Result.Success(), IsUserNotFound: true, WasUpdated: false);

        if (!Enum.TryParse<SubscriptionTier>(tierName, ignoreCase: false, out var tier))
            return new SubscriptionTierUpdateResult(Result.Failure(["Invalid subscription tier."]),
                IsUserNotFound: false,
                WasUpdated: false);

        if (user.SubscriptionTier == tier)
            return new SubscriptionTierUpdateResult(Result.Success(), IsUserNotFound: false, WasUpdated: false);

        user.SubscriptionTier = tier;
        var result = (await userManager.UpdateAsync(user)).ToApplicationResult();
        return new SubscriptionTierUpdateResult(result, IsUserNotFound: false, WasUpdated: result.Succeeded);
    }

    private async Task<Result> DeleteUserAsync(ApplicationUser user, CancellationToken ct = default)
    {
        var result = await userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
}
