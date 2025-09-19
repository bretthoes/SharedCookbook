using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Infrastructure.Identity.RepositoryExtensions;

// TODO should move these to application
public static class CookbookInvitationExtensions
{
    public static IQueryable<CookbookInvitation> GetInvitationsForUserByStatus(
        this IQueryable<CookbookInvitation> query,
        string? userId,
        CookbookInvitationStatus status)
        => query
            .Where(invitation
                => invitation.RecipientPersonId == userId
                   && invitation.InvitationStatus == status);

    public static IQueryable<InvitationDto> OrderByMostRecentlyCreated(
        this IQueryable<InvitationDto> invitations)
        => invitations.OrderByDescending(invitation => invitation.Created);
}
