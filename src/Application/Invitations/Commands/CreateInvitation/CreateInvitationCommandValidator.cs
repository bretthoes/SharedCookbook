namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public class CreateInvitationCommandValidator : AbstractValidator<CreateInvitationCommand>
{
    public CreateInvitationCommandValidator()
    {
        RuleFor(command => command.CookbookId)
            .GreaterThan(0)
            .WithMessage("CookbookId must be greater than zero.");
    }
}
