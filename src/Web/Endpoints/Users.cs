using SharedCookbook.Application.Users.Queries.GetUser;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetUser, "{id}")
            .MapGet(GetUserByEmail, "by-email/{email}")
            .MapCustomizedIdentityApi<ApplicationUser>();
    }

    public async Task<UserDto> GetUser(ISender sender, [AsParameters] GetUserQuery query)
    {
        return await sender.Send(query);
    }

    public async Task<UserDto> GetUserByEmail(ISender sender, [AsParameters] GetUserByEmailQuery query)
    {
        return await sender.Send(query);
    }
}
