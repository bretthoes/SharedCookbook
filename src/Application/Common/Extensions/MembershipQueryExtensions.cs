using SharedCookbook.Application.Memberships.Queries;

namespace SharedCookbook.Application.Common.Extensions;

public static class MembershipQueryExtensions
{
    private static IQueryable<CookbookMembership> ForCookbookAndUser(
        this IQueryable<CookbookMembership> q, int cookbookId, string userId) =>
        q.HasCookbookId(cookbookId).ForUserId(userId);

    public static Task<CookbookMembership> SingleForCookbookAndUser(
        this IQueryable<CookbookMembership> q, int cookbookId, string userId, CancellationToken ct) =>
        q.AsNoTracking().ForCookbookAndUser(cookbookId, userId).SingleAsync(ct);
    public static IQueryable<CookbookMembership> HasCookbookId(
        this IQueryable<CookbookMembership> query, int cookbookId) =>
        query.Where(membership => membership.CookbookId == cookbookId);

    public static IQueryable<MembershipDto> OrderByName(
        this IQueryable<MembershipDto> query)
        => query.OrderByDescending(dto => dto.Name);
    
    public static Task<bool> ExistsFor(
        this IQueryable<CookbookMembership> query,
        int cookbookId,
        string userId,
        CancellationToken cancellationToken) 
        => query.HasCookbookId(cookbookId).ForUserId(userId).AsNoTracking().AnyAsync(cancellationToken);
    
    private static IQueryable<CookbookMembership> ForUserId(
        this IQueryable<CookbookMembership> query, string userId) =>
        query.Where(membership => membership.CreatedBy == userId);
    
    public static Task<bool> IsMember(
        this IQueryable<CookbookMembership> memberships,
        int cookbookId,
        string personId,
        CancellationToken token = default)
        => memberships
            .AsNoTracking()
            .AnyAsync(membership => membership.CookbookId == cookbookId && membership.CreatedBy == personId, token);
}
