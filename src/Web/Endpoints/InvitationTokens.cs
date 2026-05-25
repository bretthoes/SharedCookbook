using SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;
using SharedCookbook.Application.InvitationTokens.Commands.UpdateInvitationToken;
using SharedCookbook.Application.InvitationTokens.Queries.GetInvitationToken;

namespace SharedCookbook.Web.Endpoints;

public class InvitationTokens : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(Single, pattern: "{token}").RequireAuthorization();
        builder.MapPost(Create).RequireAuthorization();
        builder.MapPut(Update, pattern: "{token}").RequireAuthorization();
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
