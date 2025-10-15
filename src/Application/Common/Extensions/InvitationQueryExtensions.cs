using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Common.Extensions;

public static class InvitationQueryExtensions
{
    public static IQueryable<CookbookInvitation> GetInvitationsForUserByStatus(
        this IQueryable<CookbookInvitation> query,
        string? userId,
        InvitationStatus status)
        => query.Where(invitation => invitation.RecipientPersonId == userId && invitation.Status == status);
    
    public static Task<int> GetInvitationsCountForUserByStatus(
        this IQueryable<CookbookInvitation> query,
        string? userId,
        InvitationStatus status,
        CancellationToken token)
        => query.CountAsync(invitation => invitation.RecipientPersonId == userId && invitation.Status == status, token);
    
    public static Task<CookbookInvitation?> FirstLinkInviteWithTokens(
        this IQueryable<CookbookInvitation> query,
        int cookbookId,
        CancellationToken cancellationToken) =>
        query
            .ForCookbook(cookbookId)
            .LinkStyle()
            .IsSent()
            .FirstOrDefaultAsync(cancellationToken);
    
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
