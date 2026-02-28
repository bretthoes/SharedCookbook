using Microsoft.AspNetCore.Identity;
using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Infrastructure.Identity;

public class SignInPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager)
    : ISignInPrincipalFactory
{
    public async Task<System.Security.Claims.ClaimsPrincipal?> CreatePrincipalForUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
        return await signInManager.CreateUserPrincipalAsync(user);
    }
}
