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
    private readonly IUser _user;

    public GetInvitationsWithPaginationQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public Task<PaginatedList<InvitationDto>> Handle(GetInvitationsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        return _context.CookbookInvitations
            .AsNoTracking()
            .Where(invitation => invitation.RecipientPersonId == _user.Id && invitation.InvitationStatus == query.Status)
            .Select(invitation => new InvitationDto {
                Id = invitation.Id,
                Created = invitation.Created,
                CreatedBy = invitation.CreatedBy,
                CookbookTitle = invitation.Cookbook == null ? "" : invitation.Cookbook.Title,
                CookbookImage = invitation.Cookbook == null ? "" : invitation.Cookbook.Image,
            })
            .OrderByDescending(c => c.Created)
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

            
    }
}
