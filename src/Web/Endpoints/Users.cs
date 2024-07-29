using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapCustomizedIdentityApi<ApplicationUser>();
    }
}
