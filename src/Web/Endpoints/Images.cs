using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Web.Endpoints;

public class Images : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.DisableAntiforgery();
        builder.MapPost(Upload).RequireAuthorization();
    }

    private static Task<string[]> Upload(ISender sender, [FromForm] IFormFileCollection files)
    {
        return sender.Send(new CreateImagesCommand(files));
    }
}
