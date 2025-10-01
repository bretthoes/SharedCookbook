using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Common;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.InvitationTokens.Commands.UpdateInvitationToken;

public sealed record UpdateInvitationTokenCommand(string Token, InvitationStatus NewStatus) : IRequest<int>;

public sealed class UpdateInvitationTokenCommandHandler(
    IApplicationDbContext context, 
    IUser user,
    IInvitationTokenFactory factory,
    TimeProvider timeProvider)
    : IRequestHandler<UpdateInvitationTokenCommand, int>
{
    public async Task<int> Handle(UpdateInvitationTokenCommand command, CancellationToken cancellationToken)
    {
        string recipientId = user.Id!;
        var link = TokenLink.Parse(command.Token);
        var token = await context.InvitationTokens.SingleById(link.TokenId, cancellationToken);
        
        Throw.IfFalse<TokenDigestMismatchException>(factory.Verify(link.Secret, token.Digest));
        Throw.IfFalse<TokenIsNotConsumableException>(token.IsRedeemable);
        
        bool alreadyMember = await context.CookbookMemberships
            .IsMember(token.CookbookId, recipientId, cancellationToken);

        bool hasPending = await context.CookbookInvitations
            .HasActiveInvite(token.CookbookId, recipientId, cancellationToken);

        Guard.Against.ExistingMembership(alreadyMember, token.CookbookId, recipientId, recipientId);
        Guard.Against.PendingInvitation(hasPending, token.CookbookId, recipientId, recipientId);
        
        if (!InvitationShouldBeUpdated(token.Status, command.NewStatus)) return token.Id; 
        
        switch (command.NewStatus)
        {
            case InvitationStatus.Accepted:
                await Accept(token, cancellationToken);
                break;
            case InvitationStatus.Rejected:
                token.Reject(timestamp: timeProvider.GetUtcNow().UtcDateTime);
                break;
            case InvitationStatus.Error:
            case InvitationStatus.Active: 
            case InvitationStatus.Revoked:
            default:
                return token.Id;
        }
        
        await context.SaveChangesAsync(cancellationToken);
        return token.Id;
    }
    
    private async Task Accept(BaseInvitation invitation, CancellationToken cancellationToken)
    {
        invitation.Accept(timestamp: timeProvider.GetUtcNow().UtcDateTime);
        bool hasMembership = await context.CookbookMemberships
            .ExistsFor(invitation.CookbookId, user.Id!, cancellationToken);
        if (!hasMembership)
        {
            var membership = CookbookMembership.GetDefaultMembership(invitation.CookbookId);
            await context.CookbookMemberships.AddAsync(membership, cancellationToken);
        }
    }
    
    private static bool InvitationShouldBeUpdated(InvitationStatus currentStatus, InvitationStatus newStatus)
        => currentStatus != newStatus;
}
