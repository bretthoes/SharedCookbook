namespace SharedCookbook.Application.Common.Extensions;

internal static class GetByPublicIdDbQuery
{
    public static Task<InvitationToken?> GetByPublicId(
        this IQueryable<InvitationToken> query, Guid tokenId, CancellationToken cancellationToken = default) =>
        query.Include(token => token.Cookbook)
        .SingleOrDefaultAsync(token => token.PublicId == tokenId, cancellationToken);
}
