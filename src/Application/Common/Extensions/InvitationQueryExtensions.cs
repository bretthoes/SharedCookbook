using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Common.Extensions;

public static class InvitationQueryExtensions
{
    public static IQueryable<CookbookInvitation> GetInvitationsForUserByStatus(
        this IQueryable<CookbookInvitation> query,
        string? userId,
        InvitationStatus status)
        => query.Where(invitation => invitation.RecipientPersonId == userId && invitation.Status == status);
    
    public static Task<CookbookInvitation?> FirstLinkInviteWithTokens(
        this IQueryable<CookbookInvitation> query,
        int cookbookId,
        CancellationToken ct = default) =>
        query
            .ForCookbook(cookbookId)
            .LinkStyle()
            .IsSent()
            .FirstOrDefaultAsync(ct);
    
    public static IQueryable<InvitationDto> OrderByMostRecentlyCreated(this IQueryable<InvitationDto> invitations) =>
        invitations.OrderByDescending(invitation => invitation.Created);
    
    private static IQueryable<CookbookInvitation> ForCookbook(this IQueryable<CookbookInvitation> q, int cookbookId) =>
        q.Where(invitation => invitation.CookbookId == cookbookId);

    private static IQueryable<CookbookInvitation> LinkStyle(this IQueryable<CookbookInvitation> q) =>
        q.Where(invitation => invitation.RecipientPersonId == null);

    private static IQueryable<CookbookInvitation> IsSent(this IQueryable<CookbookInvitation> q) =>
        q.Where(invitation => invitation.Status == InvitationStatus.Active);
    
    public static Task<bool> HasActiveInvite(
        this IQueryable<CookbookInvitation> invitations,
        int cookbookId,
        string recipientPersonId,
        CancellationToken ct = default)
        => invitations.HasInviteWithStatus(cookbookId, recipientPersonId, InvitationStatus.Active, ct);

    private static Task<bool> HasInviteWithStatus(
        this IQueryable<CookbookInvitation> invitations,
        int cookbookId,
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
