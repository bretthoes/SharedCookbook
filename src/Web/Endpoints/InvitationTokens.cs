using SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;
using SharedCookbook.Application.InvitationTokens.Commands.UpdateInvitationToken;
using SharedCookbook.Application.InvitationTokens.Queries.GetInvitationToken;

namespace SharedCookbook.Web.Endpoints;

public class InvitationTokens : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(Single, pattern: "{token}")
            .RequireAuthorization()
            .Produces<InvitationDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        builder.MapPost(Create)
            .RequireAuthorization()
            .Produces<InvitationTokenDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        builder.MapPut(Update, pattern: "{token}")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static Task<InvitationDto> Single(
        ISender sender,
        [FromRoute] string token,
        CancellationToken ct = default) =>
        sender.Send(new GetInvitationTokenQuery(token), ct);

    private static Task<InvitationTokenDto> Create(
        ISender sender,
        [FromBody] CreateInvitationTokenCommand command,
        CancellationToken ct = default) =>
        sender.Send(command, ct);

    private static async Task<IResult> Update(
        ISender sender,
        [FromRoute] string token,
        [FromBody] UpdateInvitationTokenCommand command,
        CancellationToken ct = default)
    {
        await sender.Send(command, ct);
        return Results.NoContent();
    }
}
