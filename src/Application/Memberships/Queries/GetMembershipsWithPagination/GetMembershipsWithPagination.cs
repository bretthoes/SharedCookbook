namespace SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

public record GetMembershipsWithPaginationQuery : IRequest<PaginatedList<MembershipDto>>
{
    public required int CookbookId { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetMembershipsWithPaginationQueryHandler : IRequestHandler<GetMembershipsWithPaginationQuery, PaginatedList<MembershipDto>>
{
    private readonly IIdentityUserRepository _identityService;

    public GetMembershipsWithPaginationQueryHandler(IIdentityUserRepository identityService)
    {
        _identityService = identityService;
    }

    public Task<PaginatedList<MembershipDto>> Handle(GetMembershipsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return _identityService.GetMemberships(request, cancellationToken);
    }
}

