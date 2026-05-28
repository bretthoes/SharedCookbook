using Microsoft.AspNetCore.Identity;
using SharedCookbook.Application.Users.Commands.LoginWithApple;
using SharedCookbook.Application.Users.Commands.LoginWithFacebook;
using SharedCookbook.Application.Users.Commands.LoginWithGoogle;
using SharedCookbook.Application.Users.Commands.UpdateUser;
using SharedCookbook.Application.Users.Queries;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapPost(Update, pattern: "/update")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        builder.MapGet(GetDisplayName, pattern: "/display-name")
            .Produces<DisplayNameDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        builder.MapPost(LoginGoogle, pattern: "/login-google")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        builder.MapPost(LoginApple, pattern: "/login-apple")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        builder.MapPost(LoginFacebook, pattern: "/login-facebook")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        builder.MapIdentityApi<ApplicationUser>();
    }

    private static async Task<IResult> LoginGoogle(
        ISender sender,
        [FromBody] LoginWithGoogleCommand command,
        CancellationToken ct = default)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded
            ? Results.SignIn(result.Value!, authenticationScheme: IdentityConstants.BearerScheme)
            : Results.Unauthorized();
    }

    private static async Task<IResult> LoginApple(
        ISender sender,
        [FromBody] LoginWithAppleCommand command,
        CancellationToken ct = default)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded
            ? Results.SignIn(result.Value!, authenticationScheme: IdentityConstants.BearerScheme)
            : Results.Unauthorized();
    }

    private static async Task<IResult> LoginFacebook(
        ISender sender,
        [FromBody] LoginWithFacebookCommand command,
        CancellationToken ct = default)
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
}
