using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.InvitationTokens.Commands.UpdateInvitationToken;

public sealed record UpdateInvitationTokenCommand(string Token, InvitationStatus NewStatus) : IRequest<int>;

public sealed class UpdateInvitationTokenCommandHandler(
    IApplicationDbContext context, 
    IUser user,
    IInvitationTokenFactory factory,
    IInvitationResponder responder)
    : IRequestHandler<UpdateInvitationTokenCommand, int>
{
    public async Task<int> Handle(UpdateInvitationTokenCommand command, CancellationToken cancellationToken)
    {
        string recipientId = user.Id!;
        var link = TokenLink.Parse(command.Token);
        var token = await context.InvitationTokens.SingleById(link.TokenId, cancellationToken);
        
        Throw.IfFalse<TokenDigestMismatchException>(factory.Verify(link.Secret, token.Digest));
        Throw.IfFalse<TokenIsNotConsumableException>(token.IsRedeemable);
        
        return await responder.Respond(token, command.NewStatus, recipientId, cancellationToken);
    }
}
