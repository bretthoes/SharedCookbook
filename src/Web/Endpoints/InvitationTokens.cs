using SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;
using SharedCookbook.Application.InvitationTokens.Commands.UpdateInvitationToken;
using SharedCookbook.Application.InvitationTokens.Queries.GetInvitationToken;

namespace SharedCookbook.Web.Endpoints;

public class InvitationTokens : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(Single, pattern: "{token}").RequireAuthorization();
        builder.MapPost(CreateInvitationToken).RequireAuthorization();
        builder.MapPut(Update, pattern: "{token}").RequireAuthorization();
    }
    
    private static Task<InvitationDto> Single(ISender sender, [FromRoute] string token)
        => sender.Send(new GetInvitationTokenQuery(token));

    private static Task<string> CreateInvitationToken(ISender sender, [FromBody] CreateInvitationTokenCommand command)
    {
        return sender.Send(command);
    }
    
    private static async Task<IResult> Update(ISender sender, [FromRoute] string token,
        [FromBody] UpdateInvitationTokenCommand command)
    {
        await sender.Send(command);
        return Results.NoContent();
    }
}
