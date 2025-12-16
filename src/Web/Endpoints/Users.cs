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
        builder.MapIdentityApi<ApplicationUser>();
    }

    private static async Task<IResult> Update(ISender sender, [FromBody] UpdateUserCommand command)
    {
        await sender.Send(command);
        return Results.NoContent();
    }

    private static async Task<string?> GetDisplayName(ISender sender) => await sender.Send(new GetDisplayNameQuery());
}
