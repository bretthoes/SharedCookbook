using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Common;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Infrastructure;


public sealed class InvitationResponder(
    IApplicationDbContext db,
    TimeProvider clock) : IInvitationResponder
{
    public async Task<int> Respond(BaseInvitation invite, InvitationStatus decision, string userId, CancellationToken ct)
    {
        if (!ShouldUpdate(invite.Status, decision)) return invite.Id;

        var now = clock.GetUtcNow().UtcDateTime;

        switch (decision)
        {
            case InvitationStatus.Accepted:
                invite.Accept(now);
                await EnsureMembership(invite.CookbookId, userId, ct);
                break;

            case InvitationStatus.Rejected:
                invite.Reject(now);
                break;

            default:
                return invite.Id;
        }

        await db.SaveChangesAsync(ct);
        return invite.Id;
    }

    private static bool ShouldUpdate(InvitationStatus current, InvitationStatus next) => current != next;

    private async Task EnsureMembership(int cookbookId, string userId, CancellationToken ct)
    {
        bool exists = await db.CookbookMemberships.ExistsFor(cookbookId, userId, ct);
        if (!exists)
        {
            var membership = CookbookMembership.GetDefaultMembership(cookbookId);
            await db.CookbookMemberships.AddAsync(membership, ct);
        }
    }
}
