using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Common;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Infrastructure;

public sealed class InvitationResponder(
    IApplicationDbContext context,
    TimeProvider clock) : IInvitationResponder
{
    public async Task<int> Respond(BaseInvitation invite,
        InvitationStatus decision,
        CancellationToken cancellationToken)
    {
        if (invite.Status == decision) return invite.Id;

        var now = clock.GetUtcNow().UtcDateTime;

        switch (decision)
        {
            case InvitationStatus.Accepted:
                invite.Accept(now);
                break;
            case InvitationStatus.Rejected:
                invite.Reject(now);
                break;
            case InvitationStatus.Error:
            case InvitationStatus.Active:
            case InvitationStatus.Revoked:
            default:
                return invite.Id;
        }

        await context.SaveChangesAsync(cancellationToken);
        return invite.Id;
    }
}
