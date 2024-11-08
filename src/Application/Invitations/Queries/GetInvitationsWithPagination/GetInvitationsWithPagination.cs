using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
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
    private readonly IApplicationDbContext _context;
    private readonly IIdentityUserRepository _repository;
    private readonly IUser _user;

    public GetInvitationsWithPaginationQueryHandler(IApplicationDbContext context, IIdentityUserRepository repository, IUser user)
    {
        _context = context;
        _repository = repository;
        _user = user;
    }

    public Task<PaginatedList<InvitationDto>> Handle(GetInvitationsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        return _repository.GetInvitations(query, cancellationToken);
    }
}
