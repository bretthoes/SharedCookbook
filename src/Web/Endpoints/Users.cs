using Microsoft.AspNetCore.Identity;
using SharedCookbook.Application.Users.Commands.LoginWithApple;
using SharedCookbook.Application.Users.Commands.LoginWithGoogle;
using SharedCookbook.Application.Users.Commands.UpdateUser;
using SharedCookbook.Application.Users.Queries;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapPost(Update, pattern: "/update");
        builder.MapGet(GetDisplayName, pattern: "/display-name");
        builder.MapPost(LoginGoogle, pattern: "/login-google");
        builder.MapPost(LoginApple, pattern: "/login-apple");
        builder.MapIdentityApi<ApplicationUser>();
    }

    private static async Task<IResult> LoginGoogle(ISender sender, [FromBody] LoginWithGoogleCommand command)
    {
        var result = await sender.Send(command);
        return result.Succeeded
            ? Results.SignIn(result.Value!, authenticationScheme: IdentityConstants.BearerScheme)
            : Results.Unauthorized();
    }

    private static async Task<IResult> LoginApple(ISender sender, [FromBody] LoginWithAppleCommand command)
    {
        var result = await sender.Send(command);
        return result.Succeeded
            ? Results.SignIn(result.Value!, authenticationScheme: IdentityConstants.BearerScheme)
            : Results.Unauthorized();
    }

    private static async Task<IResult> Update(ISender sender, [FromBody] UpdateUserCommand command)
    {
        await sender.Send(command);
        return Results.NoContent();
    }

    private static Task<DisplayNameDto> GetDisplayName(ISender sender) => sender.Send(new GetDisplayNameQuery());
}
