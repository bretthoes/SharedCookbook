using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Common.Security;

namespace SharedCookbook.Application.Users.Commands.LoginWithFacebook;

[AllowAnonymous]
public record LoginWithFacebookCommand(string AccessToken) : IRequest<Result<System.Security.Claims.ClaimsPrincipal>>;

public class LoginWithFacebookCommandHandler(
    IExternalLoginService externalLoginService,
    ISignInPrincipalFactory signInPrincipalFactory)
    : IRequestHandler<LoginWithFacebookCommand, Result<System.Security.Claims.ClaimsPrincipal>>
{
    public async Task<Result<System.Security.Claims.ClaimsPrincipal>> Handle(
        LoginWithFacebookCommand request,
        CancellationToken ct = default)
    {
        var loginResult = await externalLoginService.LoginWithFacebookAsync(request.AccessToken, ct);
        if (!loginResult.Succeeded)
            return Result<System.Security.Claims.ClaimsPrincipal>.Failure(loginResult.Errors);

        var principal = await signInPrincipalFactory.CreatePrincipalForUserIdAsync(loginResult.Value!, ct);
        if (principal == null)
            return Result<System.Security.Claims.ClaimsPrincipal>.Failure(["User not found."]);

        return Result<System.Security.Claims.ClaimsPrincipal>.Success(principal);
    }
}
