using SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;
using SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IIdentityRepository
{
    Task<PaginatedList<MembershipDto>> GetMemberships(
        GetMembershipsWithPaginationQuery query,
        CancellationToken ct = default);

    Task<PaginatedList<InvitationDto>> GetInvitations(
        GetInvitationsWithPaginationQuery query,
        CancellationToken ct = default);

    Task<PaginatedList<CookbookBriefDto>> GetCookbooks(
        GetCookbooksWithPaginationQuery query,
        CancellationToken ct = default);

    Task<PaginatedList<NotificationDto>> GetNotifications(
        GetNotificationsWithPaginationQuery query,
        CancellationToken ct = default);

    Task<NotificationDto?> GetLatestNotificationAsync(CancellationToken ct = default);
}
