using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }
    
    public async Task<UserDto?> FindByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null
            ? null
            : MapApplicationUserToUserDto(user);
    }
    
    public async Task<UserDto?> FindByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        return user == null
            ? null
            : MapApplicationUserToUserDto(user);
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }
    
    public async Task<string?> GetDisplayNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return !string.IsNullOrWhiteSpace(user?.DisplayName)
            ? user.DisplayName
            : user?.UserName;
    }
    
    public async Task<string?> GetEmailAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.Email;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> UpdateUserAsync(string userId, string displayName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user is null) return Result.Failure(["User not found."]);

        user.DisplayName = displayName;

        var result = await _userManager.UpdateAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
    
    private static UserDto MapApplicationUserToUserDto(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email ?? string.Empty,
            DisplayName = user.DisplayName
        };
    }
}
