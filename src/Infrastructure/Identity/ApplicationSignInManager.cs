using System.Net.Mail;
using Microsoft.AspNetCore.Identity;

namespace SharedCookbook.Infrastructure.Identity;
public class ApplicationSignInManager : SignInManager<ApplicationUser>
{
    public ApplicationSignInManager(UserManager<ApplicationUser> userManager, 
        Microsoft.AspNetCore.Http.IHttpContextAccessor contextAccessor, 
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, 
        Microsoft.Extensions.Options.IOptions<IdentityOptions> optionsAccessor, 
        Microsoft.Extensions.Logging.ILogger<SignInManager<ApplicationUser>> logger, 
        Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider schemes,
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
