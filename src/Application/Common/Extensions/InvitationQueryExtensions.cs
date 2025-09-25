using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Common.Extensions;

public static class InvitationQueryExtensions
{
    public static Task<CookbookInvitation?> FirstLinkInviteWithTokens(
        this IQueryable<CookbookInvitation> query,
        int cookbookId,
        CancellationToken cancellationToken) =>
        query
            .ForCookbook(cookbookId)
            .LinkStyle()
            .IsSent()
            .WithTokens()
            .FirstOrDefaultAsync(cancellationToken);

    private static IQueryable<CookbookInvitation> ForCookbook(
        this IQueryable<CookbookInvitation> q, int cookbookId) =>
        q.Where(invitation => invitation.CookbookId == cookbookId);

    private static IQueryable<CookbookInvitation> LinkStyle(
        this IQueryable<CookbookInvitation> q) =>
        q.Where(invitation => invitation.RecipientPersonId == null);

    private static IQueryable<CookbookInvitation> IsSent(
        this IQueryable<CookbookInvitation> q) =>
        q.Where(invitation => invitation.InvitationStatus == CookbookInvitationStatus.Sent);

    private static IQueryable<CookbookInvitation> WithTokens(
        this IQueryable<CookbookInvitation> q) =>
        q.Include(navigationPropertyPath: invitation => invitation.Tokens);
}
