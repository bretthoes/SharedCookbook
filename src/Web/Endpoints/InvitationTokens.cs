using SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;
using SharedCookbook.Application.InvitationTokens.Queries.GetInvitationToken;

namespace SharedCookbook.Web.Endpoints;

public class InvitationTokens : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(Single).RequireAuthorization();
        builder.MapPost(CreateInvitationToken).RequireAuthorization();
    }
    
    private static Task<InvitationDto> Single(ISender sender, [AsParameters] GetInvitationTokenQuery query)
        => sender.Send(query);

    private static Task<string> CreateInvitationToken(ISender sender, [FromBody] CreateInvitationTokenCommand command)
    {
        return sender.Send(command);
    }
}
