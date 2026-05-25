using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Common.Security;

namespace SharedCookbook.Application.Users.Commands.LoginWithApple;

[AllowAnonymous]
public record LoginWithAppleCommand(string IdentityToken) : IRequest<Result<System.Security.Claims.ClaimsPrincipal>>;

public class LoginWithAppleCommandHandler(
    IExternalLoginService externalLoginService,
    ISignInPrincipalFactory signInPrincipalFactory)
    : IRequestHandler<LoginWithAppleCommand, Result<System.Security.Claims.ClaimsPrincipal>>
{
    public async Task<Result<System.Security.Claims.ClaimsPrincipal>> Handle(
        LoginWithAppleCommand request,
        CancellationToken ct = default)
    {
        var loginResult = await externalLoginService.LoginWithAppleAsync(request.IdentityToken, ct);
        if (!loginResult.Succeeded)
            return Result<System.Security.Claims.ClaimsPrincipal>.Failure(loginResult.Errors);

        var principal = await signInPrincipalFactory.CreatePrincipalForUserIdAsync(loginResult.Value!, ct);
        if (principal == null)
            return Result<System.Security.Claims.ClaimsPrincipal>.Failure(["User not found."]);

        return Result<System.Security.Claims.ClaimsPrincipal>.Success(principal);
    }
}
