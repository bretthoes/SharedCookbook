namespace SharedCookbook.Application.InvitationTokens.Queries.GetInvitationToken;

public class GetInvitationTokenQueryValidator : AbstractValidator<GetInvitationTokenQuery>
{
    public GetInvitationTokenQueryValidator()
    {
        RuleFor(query => query.Token)
            .NotEmpty().WithMessage("Token cannot be empty.");
    }
}
