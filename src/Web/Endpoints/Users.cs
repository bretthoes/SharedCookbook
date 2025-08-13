using Microsoft.AspNetCore.Mvc;
using SharedCookbook.Application.Users.Commands.UpdateUser;
using SharedCookbook.Application.Users.Queries;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(UpdateUser, pattern: "/update")
            .MapGet(GetDisplayName, pattern: "/display-name")
            .MapIdentityApi<ApplicationUser>();
    }

    private static async Task<IResult> UpdateUser(ISender sender, [FromBody] UpdateUserCommand command)
    {
        await sender.Send(command);
        return Results.NoContent();
    }

    private static async Task<string?> GetDisplayName(ISender sender)
    {
        return await sender.Send(new GetDisplayNameQuery());
    }
}
