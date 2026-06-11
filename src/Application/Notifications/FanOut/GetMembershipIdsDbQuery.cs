namespace SharedCookbook.Application.Notifications.FanOut;

internal static class GetMembershipIdsDbQuery
{
    extension(IQueryable<CookbookMembership> query)
    {
        internal Task<List<string>> QueryUserIdsByCookbook(
            Guid cookbookId,
            HashSet<string> excludeUserIds,
            CancellationToken ct = default)
            => query
                .ForCookbook(cookbookId)
                .WithCreator()
                .NotCreatedByAny(excludeUserIds)
                .Select(membership => membership.CreatedBy!)
                .Distinct()
                .ToListAsync(ct);

        private IQueryable<CookbookMembership> ForCookbook(Guid cookbookId) =>
            query.Where(membership => membership.CookbookId == cookbookId);

        private IQueryable<CookbookMembership> WithCreator() =>
            query.Where(membership => membership.CreatedBy != null);

        private IQueryable<CookbookMembership> NotCreatedByAny(HashSet<string> excludeUserIds) =>
            query.Where(membership => !excludeUserIds.Contains(membership.CreatedBy!));
    }
}
