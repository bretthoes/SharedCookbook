namespace SharedCookbook.Application.Invitations.Queries.ExistsFor;

internal static class ExistsForDbQuery
{
    extension(IQueryable<CookbookMembership> query)
    {
        internal Task<bool> ExistsFor(
            Guid cookbookId,
            string userId,
            CancellationToken ct = default)
            => query
                .ForCookbookAndUser(cookbookId, userId)
                .AsNoTracking()
                .AnyAsync(ct);

        private IQueryable<CookbookMembership> ForCookbookAndUser(Guid cookbookId, string userId) =>
            query.Where(membership =>
                membership.CookbookId == cookbookId && membership.CreatedBy == userId);
    }
}
