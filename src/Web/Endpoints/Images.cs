using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Web.Endpoints;

public class Images : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .RequireAuthorization()
            .MapPost(CreateImages);
    }

    public Task<string[]> CreateImages(ISender sender, IFormFileCollection files)
    {
        return sender.Send(new CreateImagesCommand(files));
    }
}
