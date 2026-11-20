namespace SharedCookbook.Application.Common.Extensions;

public static class InvitationQueryExtensions
{
    public static IQueryable<CookbookInvitation> GetInvitationsForUserByStatus(
        this IQueryable<CookbookInvitation> query,
        string? userId,
        InvitationStatus status)
        => query.Where(invitation => invitation.RecipientPersonId == userId && invitation.Status == status);
    
    public static IQueryable<InvitationDto> OrderByMostRecentlyCreated(this IQueryable<InvitationDto> invitations) =>
        invitations.OrderByDescending(invitation => invitation.Created);
    
    public static Task<bool> HasActiveInvite(
        this IQueryable<CookbookInvitation> invitations,
        Guid cookbookId,
        string recipientPersonId,
        CancellationToken ct = default)
        => invitations.HasInviteWithStatus(cookbookId, recipientPersonId, InvitationStatus.Active, ct);

    private static Task<bool> HasInviteWithStatus(
        this IQueryable<CookbookInvitation> invitations,
        Guid cookbookId,
        string recipientPersonId,
        InvitationStatus status,
        CancellationToken ct = default)
        => invitations
            .AsNoTracking()
            .AnyAsync(invitation =>
                invitation.CookbookId == cookbookId
                && invitation.RecipientPersonId == recipientPersonId
                && invitation.Status == status, ct);
}
