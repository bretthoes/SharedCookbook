using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Users.Commands.DeleteAccount;
using SharedCookbook.Application.Users.Commands.ExternalLogin;
using SharedCookbook.Application.Users.Commands.UpdateUser;
using SharedCookbook.Application.Users.Queries;
using SharedCookbook.Infrastructure.Identity;
using SharedCookbook.Web.Infrastructure.RateLimiting;

namespace SharedCookbook.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapPost(Update, pattern: "/update")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        builder.MapDelete(DeleteAccount, pattern: "/account")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        builder.MapGet(GetDisplayName, pattern: "/display-name")
            .Produces<DisplayNameDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        builder.MapPost(LoginExternal, pattern: "/login-external")
            .Produces<AccessTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        builder.MapIdentityApi<ApplicationUser>()
            .RequireRateLimiting(RateLimitPolicyNames.AccountEmail);
    }

    private static Task<IResult> LoginExternal(
        ISender sender,
        [FromBody] ExternalLoginCommand command,
        CancellationToken ct = default) =>
        SignInAsync(sender, command, ct);

    private static async Task<IResult> SignInAsync(
        ISender sender,
        IRequest<Result<ClaimsPrincipal>> command,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded
            ? Results.SignIn(result.Value!, authenticationScheme: IdentityConstants.BearerScheme)
            : Results.Unauthorized();
    }

    private static async Task<IResult> Update(
        ISender sender,
        [FromBody] UpdateUserCommand command,
        CancellationToken ct = default)
    {
        await sender.Send(command, ct);
        return Results.NoContent();
    }

    private static Task<DisplayNameDto> GetDisplayName(ISender sender, CancellationToken ct = default) =>
        sender.Send(new GetDisplayNameQuery(), ct);

    private static async Task<IResult> DeleteAccount(ISender sender, CancellationToken ct = default)
    {
        await sender.Send(new DeleteAccountCommand(), ct);
        return Results.NoContent();
    }
}
