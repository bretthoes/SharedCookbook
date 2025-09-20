using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationPreview;

public record GetInvitationPreviewQuery(string Token) : IRequest<InvitationDto>;

public class GetInvitationPreviewQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetInvitationPreviewQuery, InvitationDto>
{
    public async Task<InvitationDto> Handle(GetInvitationPreviewQuery request, CancellationToken cancellationToken)
    {
        // TODO refine this 
        if (!TokenLink.TryParse(request.Token, out var parsed))
            throw new Exception();

        var invitation = await context.CookbookInvitations
            .AsNoTracking()
            .Include(i => i.Cookbook)
            .FirstOrDefaultAsync(i =>
                i.Id == parsed.TokenId &&
                i.InvitationStatus == CookbookInvitationStatus.Sent, cancellationToken);
        
        Guard.Against.NotFound(parsed.TokenId, invitation);

        //if (!tokens.Verify(code, new InvitationCodeHash(invitation.Hash, invitation.Salt)))
//            throw new NotFoundException(key: parsed.InvitationId.ToString(), nameof(CookbookInvitation));

        return new InvitationDto
        {
            CookbookTitle = invitation.Cookbook?.Title ?? ""
        };
    }
}

public class GetInvitationPreviewQueryValidator : AbstractValidator<GetInvitationPreviewQuery>
{
    public GetInvitationPreviewQueryValidator()
    {
        RuleFor(query => query.Token)
            .NotEmpty().WithMessage("Token cannot be empty.");
    }
}
