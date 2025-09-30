using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public static class CookbookQueryExtensions
{
    public static Task<bool> IsMember(
        this IQueryable<CookbookMembership> memberships,
        int cookbookId,
        string personId,
        CancellationToken token = default)
        => memberships
            .AsNoTracking()
            .AnyAsync(membership => membership.CookbookId == cookbookId && membership.CreatedBy == personId, token);

    public static Task<bool> HasActiveInvite(
        this IQueryable<CookbookInvitation> invitations,
        int cookbookId,
        string recipientPersonId,
        CancellationToken token = default)
        => invitations.HasInviteWithStatus(
            cookbookId, recipientPersonId, token, statuses: InvitationStatus.Active);

    private static Task<bool> HasInviteWithStatus(
        this IQueryable<CookbookInvitation> invitations,
        int cookbookId,
        string recipientPersonId,
        CancellationToken token = default,
        params InvitationStatus[] statuses)
        => invitations
            .AsNoTracking()
            .AnyAsync(invitation =>
                invitation.CookbookId == cookbookId
                && invitation.RecipientPersonId == recipientPersonId
                && statuses.Contains(invitation.Status), token);
}
