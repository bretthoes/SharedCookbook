using Microsoft.Extensions.Options;
using SharedCookbook.Application.Images.Commands.CreateImages;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

public sealed record GetInvitationsWithPaginationQuery(
    InvitationStatus Status = InvitationStatus.Active,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<PaginatedList<InvitationDto>>;

public sealed class GetInvitationsWithPaginationQueryHandler(
    IApplicationDbContext context,
    IUser user,
    IOptions<ImageUploadOptions> options)
    : IRequestHandler<GetInvitationsWithPaginationQuery, PaginatedList<InvitationDto>>
{
    public Task<PaginatedList<InvitationDto>> Handle(
        GetInvitationsWithPaginationQuery query,
        CancellationToken ct = default)
        => context.CookbookInvitations
            .AsNoTracking()
            .QueryDtos(user.Id!, query.Status, options.Value.ImageBaseUrl, query.PageNumber, query.PageSize, ct);
}
