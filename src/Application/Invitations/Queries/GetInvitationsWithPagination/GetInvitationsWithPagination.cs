using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

public record GetInvitationsWithPaginationQuery : IRequest<PaginatedList<InvitationDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetInvitationsWithPaginationQueryHandler : IRequestHandler<GetInvitationsWithPaginationQuery, PaginatedList<InvitationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetInvitationsWithPaginationQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public Task<PaginatedList<InvitationDto>> Handle(GetInvitationsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return _context.CookbookInvitations
            .AsNoTracking()
            .Where(invitation => invitation.RecipientPersonId == _user.Id)
            .Select(invitation => new InvitationDto {
                Id = invitation.Id,
                Created = invitation.Created,
                CreatedBy = invitation.CreatedBy,
                CookbookTitle = invitation.Cookbook == null ? "" : invitation.Cookbook.Title,
                CookbookImage = invitation.Cookbook == null ? "" : invitation.Cookbook.Image,
            })
            .OrderByDescending(c => c.Created)
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

            
    }
}

// TODO move
public class InvitationDto
{
    public int Id { get; set; }

    public string? SenderName { get; set; } = string.Empty;

    public required string CookbookTitle { get; set; }

    public string? CookbookImage { get; set; }

    public DateTimeOffset Created { get; set; }

    public int? CreatedBy { get; set; }
}
