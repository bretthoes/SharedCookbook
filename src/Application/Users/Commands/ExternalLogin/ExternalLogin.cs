using System.Security.Claims;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Common.Security;

namespace SharedCookbook.Application.Users.Commands.ExternalLogin;

[AllowAnonymous]
public sealed record LoginWithGoogleCommand(string IdToken) : IRequest<Result<ClaimsPrincipal>>;

[AllowAnonymous]
public sealed record LoginWithAppleCommand(string IdentityToken) : IRequest<Result<ClaimsPrincipal>>;

[AllowAnonymous]
public sealed record LoginWithFacebookCommand(string AccessToken) : IRequest<Result<ClaimsPrincipal>>;

public sealed class ExternalLoginCommandHandler(
    IExternalLoginService externalLoginService,
    ISignInPrincipalFactory signInPrincipalFactory)
    : IRequestHandler<LoginWithGoogleCommand, Result<ClaimsPrincipal>>,
        IRequestHandler<LoginWithAppleCommand, Result<ClaimsPrincipal>>,
        IRequestHandler<LoginWithFacebookCommand, Result<ClaimsPrincipal>>
{
    public Task<Result<ClaimsPrincipal>> Handle(LoginWithGoogleCommand request, CancellationToken ct = default) =>
        SignInAsync(externalLoginService.LoginWithGoogleAsync(request.IdToken, ct), ct);

    public Task<Result<ClaimsPrincipal>> Handle(LoginWithAppleCommand request, CancellationToken ct = default) =>
        SignInAsync(externalLoginService.LoginWithAppleAsync(request.IdentityToken, ct), ct);

    public Task<Result<ClaimsPrincipal>> Handle(LoginWithFacebookCommand request, CancellationToken ct = default) =>
        SignInAsync(externalLoginService.LoginWithFacebookAsync(request.AccessToken, ct), ct);

    private async Task<Result<ClaimsPrincipal>> SignInAsync(Task<Result<string>> login, CancellationToken ct)
    {
        var loginResult = await login;
        if (!loginResult.Succeeded)
            return Result<ClaimsPrincipal>.Failure(loginResult.Errors);

        var principal = await signInPrincipalFactory.CreatePrincipalForUserIdAsync(loginResult.Value!, ct);
        return principal is null
            ? Result<ClaimsPrincipal>.Failure(["User not found."])
            : Result<ClaimsPrincipal>.Success(principal);
    }
}
