using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Common;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Infrastructure;


public sealed class InvitationResponder(
    IApplicationDbContext context,
    TimeProvider clock) : IInvitationResponder
{
    public async Task<int> Respond(BaseInvitation invite,
        InvitationStatus decision,
        string userId,
        CancellationToken cancellationToken)
    {
        if (!ShouldUpdate(current: invite.Status, next: decision)) return invite.Id;

        var now = clock.GetUtcNow().UtcDateTime;

        switch (decision)
        {
            case InvitationStatus.Accepted:
                invite.Accept(now);
                await EnsureMembership(invite.CookbookId, userId, cancellationToken);
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

    private static bool ShouldUpdate(InvitationStatus current, InvitationStatus next) => current != next;

    private async Task EnsureMembership(int cookbookId, string userId, CancellationToken ct)
    {
        bool exists = await context.CookbookMemberships.ExistsFor(cookbookId, userId, ct);
        if (!exists)
        {
            var membership = CookbookMembership.NewDefault(cookbookId);
            await context.CookbookMemberships.AddAsync(membership, ct);
        }
    }
}
