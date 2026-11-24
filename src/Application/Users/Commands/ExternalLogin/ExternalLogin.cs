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
    public async Task<Result<ClaimsPrincipal>> Handle(ExternalLoginCommand request, CancellationToken ct = default)
    {
        var provider = providers.FirstOrDefault(candidate => candidate.Provider == request.Provider);
        if (provider is null)
            return Result<ClaimsPrincipal>.Failure(["Provider must be Google, Apple, or Facebook."]);

        var login = await provider.LoginAsync(request.Token, ct);
        if (!login.Succeeded)
            return Result<ClaimsPrincipal>.Failure(login.Errors);

        var userId = login.Value!;
        var principal = await signInPrincipalFactory.CreatePrincipalForUserIdAsync(userId, ct);
        if (principal is null)
            return Result<ClaimsPrincipal>.Failure(["User not found."]);

        return Result<ClaimsPrincipal>.Success(principal);
    }
}
