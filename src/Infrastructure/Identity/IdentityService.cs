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
    public async Task<Application.Common.Interfaces.Identity?> FindByEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        
        return user == null ? null : new Application.Common.Interfaces.Identity(user.Email, user.DisplayName);
    }

    public async Task<string?> GetIdByEmailAsync(string email)
        => (await userManager.FindByEmailAsync(email))?.Id;
    
    public async Task<Application.Common.Interfaces.Identity?> FindByIdAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        return user == null ? null : new Application.Common.Interfaces.Identity(user.Email, user.DisplayName);
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        return user?.UserName;
    }
    
    public async Task<string?> GetDisplayNameAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        return !string.IsNullOrWhiteSpace(user?.DisplayName)
            ? user.DisplayName
            : user?.UserName;
    }
    
    public async Task<string?> GetEmailAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        return user?.Email;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);

        return user != null && await userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
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

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> UpdateUserAsync(string userId, string displayName)
    {
        var user = await userManager.FindByIdAsync(userId);
        
        if (user is null) return Result.Failure(["User not found."]);

        user.DisplayName = displayName;

        var result = await userManager.UpdateAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
}
