using System.Net.Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SharedCookbook.Infrastructure.Identity;
public class ApplicationSignInManager : SignInManager<ApplicationUser>
{
    public ApplicationSignInManager(UserManager<ApplicationUser> userManager,
        IHttpContextAccessor contextAccessor, 
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, 
        IOptions<IdentityOptions> optionsAccessor, 
        ILogger<SignInManager<ApplicationUser>> logger, 
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<ApplicationUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }

    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password,
        bool isPersistent, bool lockoutOnFailure)
    {
        try
        {
            var email = new MailAddress(userName).Address;
            var user = await UserManager.FindByEmailAsync(email);
            return user == null 
                ? SignInResult.Failed 
                : await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }
        catch (FormatException)
        {
            var user = await UserManager.FindByNameAsync(userName);
            return user == null 
                ? SignInResult.Failed 
                : await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }
    }
}
