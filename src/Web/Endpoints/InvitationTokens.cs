using SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;

namespace SharedCookbook.Web.Endpoints;

public class InvitationTokens : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapPost(CreateInvitationToken).RequireAuthorization();
    }
    
    private static Task<string> CreateInvitationToken(ISender sender, CreateInvitationTokenCommand command)
        => sender.Send(command);
}
