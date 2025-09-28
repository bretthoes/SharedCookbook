namespace SharedCookbook.Application.Common.Extensions;

public static class InvitationTokenQueryExtensions
{
    public static Task<InvitationToken?> FirstByIdWithInvitation(
        this IQueryable<InvitationToken> query,
        long tokenId,
        CancellationToken cancellationToken = default) =>
        query
            .ById(tokenId)
            .WithInvitation()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

    private static IQueryable<InvitationToken> ById(this IQueryable<InvitationToken> q, long id) =>
        q.Where(invitationToken => invitationToken.Id == id);

    private static IQueryable<InvitationToken> WithInvitation(this IQueryable<InvitationToken> q) =>
        q.Include(navigationPropertyPath: invitationToken => invitationToken.Invitation);
}
