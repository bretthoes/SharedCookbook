using SharedCookbook.Application.Users.Queries.GetUser;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetUser, "{id}")
            .MapCustomizedIdentityApi<ApplicationUser>();
    }

    public async Task<UserDto> GetUser(ISender sender, [AsParameters] GetUserQuery query)
    {
        return await sender.Send(query);
    }
}
