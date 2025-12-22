using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationsCount;

internal static class GetCountByUserAndStatusDbQuery
{
    extension(IQueryable<CookbookInvitation> query)
    {
        internal Task<int> GetCountByUserAndStatus(string userId, InvitationStatus status, CancellationToken token)
        => query.CountAsync(invitation => invitation.RecipientPersonId == userId && invitation.Status == status, token);
    }
}
