namespace SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;

public class CreateInvitationTokenCommandValidator : AbstractValidator<CreateInvitationTokenCommand>
{
    public CreateInvitationTokenCommandValidator()
    {
        RuleFor(command => command.CookbookId)
            .GreaterThan(0)
            .WithMessage("CookbookId must be greater than zero.");
    }
}
