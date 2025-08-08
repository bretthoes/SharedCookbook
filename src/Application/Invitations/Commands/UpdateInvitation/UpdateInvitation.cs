using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.UpdateInvitation;

public record UpdateInvitationCommand : IRequest<int>
{
    public int Id { get; init; }

    public CookbookInvitationStatus NewStatus { get; init; }
}

public class UpdateInvitationCommandHandler(
    IApplicationDbContext context,
    IUser user,
    TimeProvider timeProvider)
    : IRequestHandler<UpdateInvitationCommand, int>
{
    public async Task<int> Handle(UpdateInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await context.CookbookInvitations.FindAsync(keyValues: [command.Id], cancellationToken);
        Guard.Against.NotFound(command.Id, invitation);
        Guard.Against.Null(user.Id);

        if (!InvitationShouldBeUpdated(invitation, command.NewStatus)) return invitation.Id; 

        switch (command.NewStatus)
        {
            case CookbookInvitationStatus.Accepted:
                invitation.Accept(timestamp: timeProvider.GetUtcNow().UtcDateTime);

                if (await UserDoesNotHaveMembershipInCookbook(invitation.CookbookId, user.Id, cancellationToken))
                {
                    var membership = CookbookMembership.GetDefaultMembership(invitation.CookbookId);
                    await context.CookbookMemberships.AddAsync(membership, cancellationToken);
                }
                break;
            case CookbookInvitationStatus.Rejected:
                invitation.Reject(timestamp: timeProvider.GetUtcNow().UtcDateTime);
                break;
            case CookbookInvitationStatus.Unknown:
            case CookbookInvitationStatus.Sent:
            default:
                return invitation.Id;
        }

        await context.SaveChangesAsync(cancellationToken);
        
        return invitation.Id;
    }

    private static bool InvitationShouldBeUpdated(CookbookInvitation invitation, CookbookInvitationStatus newStatus)
        => invitation.InvitationStatus != newStatus;
    
    private async Task<bool> UserDoesNotHaveMembershipInCookbook(
        int cookbookId,
        string userId,
        CancellationToken token) 
        => !await context.CookbookMemberships
            .AnyAsync(membership
                => membership.CookbookId == cookbookId
                   && membership.CreatedBy == userId, token);
}
