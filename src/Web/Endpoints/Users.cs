using Microsoft.AspNetCore.Mvc;
using SharedCookbook.Application.Users.Commands.UpdateUser;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(UpdateUser, "/update")
            .MapIdentityApi<ApplicationUser>();
    }
    
    public async Task<IResult> UpdateUser(ISender sender, [FromBody] UpdateUserCommand command)
    {
        await sender.Send(command);
        return Results.NoContent();
    }
}
