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
            .RequireRateLimiting(RateLimitPolicyNames.ImageUploadDaily)
            .Produces<string[]>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status429TooManyRequests);
    }

    private static Task<string[]> Upload(
        ISender sender,
        [FromForm] IFormFileCollection files,
        CancellationToken ct = default) =>
        sender.Send(new CreateImagesCommand(files), ct);
}
