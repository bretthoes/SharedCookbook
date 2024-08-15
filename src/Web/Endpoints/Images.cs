using SharedCookbook.Application.Images.Commands.CreateImage;

namespace SharedCookbook.Web.Endpoints;

public class Images : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .RequireAuthorization()
            .MapPost(CreateImage);
    }

    public async Task<IResult> CreateImage(ISender sender, IFormFile file)
    {
        var result = await sender.Send(new CreateImageCommand(file));

        return string.IsNullOrWhiteSpace(result)
            ? Results.BadRequest("Upload failed.")
            : Results.Ok(result);
    }
}
