using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationPreview;

public record GetInvitationPreviewQuery(string Token) : IRequest<InvitationDto>;

public class GetInvitationPreviewQueryHandler : IRequestHandler<GetInvitationPreviewQuery, InvitationDto>
{
    public Task<InvitationDto> Handle(GetInvitationPreviewQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class GetInvitationPreviewQueryValidator : AbstractValidator<GetInvitationPreviewQuery>
{
    public GetInvitationPreviewQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token cannot be empty.");
    }
}
