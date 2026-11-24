using System.Security.Claims;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Common.Security;

namespace SharedCookbook.Application.Users.Commands.ExternalLogin;

[AllowAnonymous]
public sealed record ExternalLoginCommand(ExternalLoginProvider Provider, string Token) : IRequest<Result<ClaimsPrincipal>>;

public sealed class ExternalLoginCommandHandler(
    IEnumerable<IExternalLoginProvider> providers,
    ISignInPrincipalFactory signInPrincipalFactory)
    : IRequestHandler<ExternalLoginCommand, Result<ClaimsPrincipal>>
{
    public Task<Result<ClaimsPrincipal>> Handle(ExternalLoginCommand request, CancellationToken ct = default) =>
        SignInAsync(LoginAsync(request, ct), ct);

    private Task<Result<string>> LoginAsync(ExternalLoginCommand request, CancellationToken ct)
    {
        var provider = providers.FirstOrDefault(candidate => candidate.Provider == request.Provider);
        return provider is null
            ? Task.FromResult(Result<string>.Failure(["Provider must be Google, Apple, or Facebook."]))
            : provider.LoginAsync(request.Token, ct);
    }

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
