namespace SharedCookbook.Application.InvitationTokens.Queries.GetInvitationToken;

internal static class GetByPublicIdDbQuery
{
    internal static Task<InvitationToken?> GetByPublicId(
        this IQueryable<InvitationToken> query, Guid tokenId, CancellationToken cancellationToken = default) =>
        query.Include(token => token.Cookbook)
        .SingleOrDefaultAsync(token => token.PublicId == tokenId, cancellationToken);
}
