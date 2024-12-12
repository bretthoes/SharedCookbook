using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

public record GetInvitationsWithPaginationQuery : IRequest<PaginatedList<InvitationDto>>
{
    public CookbookInvitationStatus Status { get; init; } = CookbookInvitationStatus.Sent;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetInvitationsWithPaginationQueryHandler : IRequestHandler<GetInvitationsWithPaginationQuery, PaginatedList<InvitationDto>>
{
    private readonly IIdentityUserRepository _repository;

    public GetInvitationsWithPaginationQueryHandler(IApplicationDbContext context, IIdentityUserRepository repository)
    {
        _repository = repository;
    }

    public Task<PaginatedList<InvitationDto>> Handle(GetInvitationsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        return _repository.GetInvitations(query, cancellationToken);
    }
}
