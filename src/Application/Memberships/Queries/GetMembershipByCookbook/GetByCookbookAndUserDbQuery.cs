namespace SharedCookbook.Application.Memberships.Queries.GetMembershipByCookbook;

internal static class GetByCookbookAndUserDbQuery
{
    extension(IQueryable<CookbookMembership> query)
    {
        internal async Task<CookbookMembership> GetByCookbookAndUser(
            Guid cookbookId,
            string userId,
            CancellationToken ct = default)
            => await query
                .Where(membership => membership.CookbookId == cookbookId && membership.CreatedBy == userId)
                .SingleOrDefaultAsync(ct)
                ?? throw new NotFoundException(key: $"{cookbookId}:{userId}", nameof(CookbookMembership));
    }
}
