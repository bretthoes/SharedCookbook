using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationPreview;

public record GetInvitationPreviewQuery(string Token) : IRequest<InvitationDto>;

public class GetInvitationPreviewQueryHandler(
    IApplicationDbContext context,
    IInvitationTokenService tokens)
    : IRequestHandler<GetInvitationPreviewQuery, InvitationDto>
{
    public async Task<InvitationDto> Handle(GetInvitationPreviewQuery request, CancellationToken cancellationToken)
    {
        // TODO refine this 
        var parsed = tokens.Parse(request.Token)
                     ?? throw new NotFoundException(nameof(CookbookInvitation), request.Token);

        var invitation = await context.CookbookInvitations
                             .AsNoTracking()
                             .Include(i => i.Cookbook)
                             .FirstOrDefaultAsync(i =>
                                 i.Id == parsed.InvitationId &&
                                 i.InvitationStatus == CookbookInvitationStatus.Sent, cancellationToken)
                         ?? throw new NotFoundException(key: parsed.InvitationId.ToString(), nameof(CookbookInvitation));

        switch (parsed)
        {
            case InvitationToken.Link(var id, var code):
                if (!tokens.Verify(code, new HashedInvitationCode(invitation.Hash, invitation.Salt)))
                    throw new NotFoundException(key: parsed.InvitationId.ToString(), nameof(CookbookInvitation));

                break;

            case InvitationToken.Email(var id):
                if (invitation.RecipientPersonId is null)
                    throw new NotFoundException(key: parsed.InvitationId.ToString(), nameof(CookbookInvitation));

                break;
        }

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
