using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;
using SharedCookbook.Application.InvitationTokens.Queries.GetInvitationToken;

namespace SharedCookbook.Web.Endpoints;

public class InvitationTokens : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(GetInvitationToken).RequireAuthorization();
        builder.MapPost(CreateInvitationToken).RequireAuthorization();
    }
    
    private static Task<InvitationDto> GetInvitationToken(ISender sender, [AsParameters] GetInvitationTokenQuery query)
        => sender.Send(query);

    private static Task<string> CreateInvitationToken(ISender sender, [FromBody] CreateInvitationTokenCommand command)
    {
        return sender.Send(command);
    }
}
