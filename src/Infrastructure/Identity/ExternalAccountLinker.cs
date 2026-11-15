using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Infrastructure.Identity;

/// <summary>
/// Resolves a validated external login (Google, Apple, Facebook) to a local account.
/// </summary>
public sealed class ExternalAccountLinker(
    UserManager<ApplicationUser> userManager,
    ILogger<ExternalAccountLinker> logger)
{
    public async Task<Result<string>> FindOrCreateUserAsync(
        string loginProvider,
        string subject,
        string email,
        bool isEmailVerified)
    {
        var user = await userManager.FindByLoginAsync(loginProvider, providerKey: subject);
        if (user is not null)
            return Result<string>.Success(user.Id);

        user = await userManager.FindByEmailAsync(email);
        if (user is not null)
            return await LinkExistingUserAsync(user, loginProvider, subject, isEmailVerified);

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
        };

        var createResult = await userManager.CreateAsync(user);
        if (!createResult.Succeeded)
            return Failure(createResult);

        var linkResult = await userManager.AddLoginAsync(
            user,
            new UserLoginInfo(loginProvider, providerKey: subject, loginProvider));
        if (linkResult.Succeeded)
            return Result<string>.Success(user.Id);

        await userManager.DeleteAsync(user);
        return Failure(linkResult);

    }

    private async Task<Result<string>> LinkExistingUserAsync(
        ApplicationUser user,
        string loginProvider,
        string subject,
        bool isEmailVerified)
    {
        if (!isEmailVerified)
        {
            logger.LogWarning(
                "{Provider} login: email matches an existing account but the provider did not verify it",
                loginProvider);
            return Result<string>.Failure([$"{loginProvider} did not verify this email address."]);
        }

        if (!user.EmailConfirmed)
        {
            var claimResult = await ClaimUnconfirmedAccountAsync(user);
            if (!claimResult.Succeeded)
                return Failure(claimResult);
        }

        var addLoginResult = await userManager.AddLoginAsync(
            user,
            new UserLoginInfo(loginProvider, providerKey: subject, loginProvider));
        return addLoginResult.Succeeded
            ? Result<string>.Success(user.Id)
            : Failure(addLoginResult);
    }

    /// <summary>
    /// The password on an unconfirmed account was set by someone who never proved they own the
    /// email. Remove it before the verified provider login takes the account over.
    /// </summary>
    private async Task<IdentityResult> ClaimUnconfirmedAccountAsync(ApplicationUser user)
    {
        if (await userManager.HasPasswordAsync(user))
        {
            var removeResult = await userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
                return removeResult;
        }

        user.EmailConfirmed = true;
        return await userManager.UpdateAsync(user);
    }

    private static Result<string> Failure(IdentityResult result) =>
        Result<string>.Failure(result.Errors.Select(error => error.Description).ToArray());
}
