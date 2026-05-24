using SharedCookbook.Application.Images.Commands.CreateImages;
using SharedCookbook.Web.Infrastructure.RateLimiting;

namespace SharedCookbook.Web.Endpoints;

public class Images : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.DisableAntiforgery();
        builder.MapPost(Upload)
            .RequireAuthorization()
            .RequireRateLimiting(RateLimitPolicyNames.ImageUploadDaily);
    }

    private static Task<string[]> Upload(ISender sender, [FromForm] IFormFileCollection files)
    {
        return sender.Send(new CreateImagesCommand(files));
    }
}
