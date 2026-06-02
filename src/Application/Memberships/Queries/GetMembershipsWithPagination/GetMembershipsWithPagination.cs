namespace SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

public sealed record GetMembershipsWithPaginationQuery : IRequest<PaginatedList<MembershipDto>>
{
    public required int CookbookId { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public sealed class GetMembershipsWithPaginationQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMembershipsWithPaginationQuery, PaginatedList<MembershipDto>>
{
    public Task<PaginatedList<MembershipDto>> Handle(GetMembershipsWithPaginationQuery request,
        CancellationToken ct = default)
        => context.CookbookMemberships
            .AsNoTracking()
            .QueryDtos(request.CookbookId, request.PageNumber, request.PageSize, ct);
}

