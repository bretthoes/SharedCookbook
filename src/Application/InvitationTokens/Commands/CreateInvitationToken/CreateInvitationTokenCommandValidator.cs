namespace SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;

public class CreateInvitationTokenCommandValidator : AbstractValidator<CreateInvitationTokenCommand>
{
    public CreateInvitationTokenCommandValidator()
    {
        RuleFor(command => command.CookbookId)
            .NotEmpty();
    }
}
