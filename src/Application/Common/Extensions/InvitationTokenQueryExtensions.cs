namespace SharedCookbook.Application.Common.Extensions;

public static class InvitationTokenQueryExtensions
{
    public static Task<InvitationToken> SingleByIdWithInvitation(
        this IQueryable<InvitationToken> query,
        Guid tokenId,
        CancellationToken cancellationToken = default) =>
        query
            .ById(tokenId)
            .WithInvitation()
            .AsNoTracking()
            .SingleAsync(cancellationToken);

    private static IQueryable<InvitationToken> ById(this IQueryable<InvitationToken> q, Guid id) =>
        q.Where(invitationToken => invitationToken.PublicId == id);

    private static IQueryable<InvitationToken> WithInvitation(this IQueryable<InvitationToken> q) =>
        q.Include(navigationPropertyPath: invitationToken => invitationToken.Invitation);
}
