using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.UpdateInvitation;

public sealed record UpdateInvitationCommand(int Id, InvitationStatus NewStatus) : IRequest<int>;

public sealed class UpdateInvitationCommandHandler(
    IApplicationDbContext context,
    IUser user,
    TimeProvider timeProvider)
    : IRequestHandler<UpdateInvitationCommand, int>
{
    public async Task<int> Handle(UpdateInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await context.CookbookInvitations.FindAsync(keyValues: [command.Id], cancellationToken);
        Guard.Against.NotFound(command.Id, invitation);
        
        if (!InvitationShouldBeUpdated(invitation, command.NewStatus)) return invitation.Id; 
        if (invitation.RecipientPersonId != user.Id) return invitation.Id; // TODO throw something here(auth?); this ensures only the recipient can accept/reject

        switch (command.NewStatus)
        {
            case InvitationStatus.Accepted:
                await Accept(invitation, cancellationToken);
                break;
            case InvitationStatus.Rejected:
                invitation.Reject(timestamp: timeProvider.GetUtcNow().UtcDateTime);
                break;
            case InvitationStatus.Error:
            case InvitationStatus.Active: 
            case InvitationStatus.Revoked:
            default:
                return invitation.Id;
        }

        await context.SaveChangesAsync(cancellationToken);
        
        return invitation.Id;
    }

    private async Task Accept(CookbookInvitation invitation, CancellationToken cancellationToken)
    {
        invitation.Accept(timestamp: timeProvider.GetUtcNow().UtcDateTime);
        Guard.Against.Null(user.Id);
        bool hasMembership = await context.CookbookMemberships
            .ExistsFor(invitation.CookbookId, user.Id, cancellationToken);
        if (!hasMembership)
        {
            var membership = CookbookMembership.GetDefaultMembership(invitation.CookbookId);
            await context.CookbookMemberships.AddAsync(membership, cancellationToken);
        }
    }

    private static bool InvitationShouldBeUpdated(CookbookInvitation invitation, InvitationStatus newStatus)
        => invitation.Status != newStatus;
}
