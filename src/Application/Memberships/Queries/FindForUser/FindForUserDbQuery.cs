namespace SharedCookbook.Application.Memberships.Queries.FindForUser;

internal static class FindForUserDbQuery
{
    extension(IQueryable<CookbookMembership> query)
    {
        internal Task<CookbookMembership?> FindForUserAsync(
            Guid cookbookId,
            string userId,
            CancellationToken ct = default)
            => query
                .ForCookbookAndUser(cookbookId, userId)
                .SingleOrDefaultAsync(ct);

        private IQueryable<CookbookMembership> ForCookbookAndUser(Guid cookbookId, string userId) =>
            query.Where(membership =>
                membership.CookbookId == cookbookId && membership.CreatedBy == userId);
    }
}
