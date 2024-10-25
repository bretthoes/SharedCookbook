namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public class CreateInvitationCommandValidator : AbstractValidator<CreateInvitationCommand>
{
    public CreateInvitationCommandValidator()
    {
        RuleFor(v => v.Email)
            .MinimumLength(6)
            .MaximumLength(256)
            .EmailAddress()
            .NotEmpty()
            .WithMessage("Email must be a valid email address.");

        RuleFor(v => v.CookbookId)
            .GreaterThan(0)
            .WithMessage("CookbookId must be greater than zero.");
    }
}
