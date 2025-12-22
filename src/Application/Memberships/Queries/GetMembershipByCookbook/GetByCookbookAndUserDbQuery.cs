namespace SharedCookbook.Application.Memberships.Queries.GetMembershipByCookbook;

internal static class GetByCookbookAndUserDbQuery
{
    extension(IQueryable<CookbookMembership> query)
    {
        internal Task<CookbookMembership> GetByCookbookAndUser(int cookbookId, string userId, CancellationToken ct) =>
            query.ForCookbookAndUser(cookbookId, userId).SingleAsync(ct);

        private IQueryable<CookbookMembership> ForCookbookAndUser(int cookbookId, string userId) =>
            query.HasCookbookId(cookbookId).ForUserId(userId);
    }
}
