using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Common.Security;

namespace SharedCookbook.Application.Users.Commands.LoginWithGoogle;

[AllowAnonymous]
public record LoginWithGoogleCommand(string IdToken) : IRequest<Result<System.Security.Claims.ClaimsPrincipal>>;

public class LoginWithGoogleCommandHandler(
    IExternalLoginService externalLoginService,
    ISignInPrincipalFactory signInPrincipalFactory)
    : IRequestHandler<LoginWithGoogleCommand, Result<System.Security.Claims.ClaimsPrincipal>>
{
    public async Task<Result<System.Security.Claims.ClaimsPrincipal>> Handle(
        LoginWithGoogleCommand request,
        CancellationToken ct = default)
    {
        var loginResult = await externalLoginService.LoginWithGoogleAsync(request.IdToken, ct);
        if (!loginResult.Succeeded)
            return Result<System.Security.Claims.ClaimsPrincipal>.Failure(loginResult.Errors);

        var principal = await signInPrincipalFactory.CreatePrincipalForUserIdAsync(loginResult.Value!, ct);
        if (principal == null)
            return Result<System.Security.Claims.ClaimsPrincipal>.Failure(["User not found."]);

        return Result<System.Security.Claims.ClaimsPrincipal>.Success(principal);
    }
}
