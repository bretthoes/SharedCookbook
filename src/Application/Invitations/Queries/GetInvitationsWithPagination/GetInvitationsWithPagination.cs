using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

public record GetInvitationsWithPaginationQuery : IRequest<PaginatedList<CookbookInvitation>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetInvitationsWithPaginationQueryHandler : IRequestHandler<GetInvitationsWithPaginationQuery, PaginatedList<CookbookInvitation>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetInvitationsWithPaginationQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public Task<PaginatedList<CookbookInvitation>> Handle(GetInvitationsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return _context.CookbookInvitations
            .AsNoTracking()
            .Where(x => x.RecipientPersonId == _user.Id)
            .OrderByDescending(c => c.Created)
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}

// TODO move
public class InvitationDto
{
    public int Id { get; set; }

    public required string SenderName { get; set; }

    public required string CookbookTitle { get; set; }

    public string? CookbookImage { get; set; }

    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; }
}
