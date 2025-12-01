using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Common;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Infrastructure;

public sealed class InvitationResponder(
    IApplicationDbContext context,
    IUser user,
    TimeProvider clock) : IInvitationResponder
{
    public async Task<int> Respond(BaseInvitation invite,
        InvitationStatus decision,
        CancellationToken cancellationToken = default)
    {
        if (StatusUnchanged(current: invite.Status, updated: decision)) return invite.Id;
        
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);

        var now = clock.GetUtcNow();

        switch (decision)
        {
            case InvitationStatus.Accepted:
                invite.Accept(now, user.Id);
                break;
            case InvitationStatus.Rejected:
                invite.Reject(now);
                break;
            case InvitationStatus.Error:
            case InvitationStatus.Active:
            case InvitationStatus.Revoked:
            case InvitationStatus.Unknown:
            default:
                throw new NotSupportedException($"Status {decision} with Id {invite.Id} not supported by user {user.Id}");
        }

        await context.SaveChangesAsync(cancellationToken);
        return invite.Id;
    }
    
    private static bool StatusUnchanged(InvitationStatus current,  InvitationStatus updated) =>  current == updated;
}
