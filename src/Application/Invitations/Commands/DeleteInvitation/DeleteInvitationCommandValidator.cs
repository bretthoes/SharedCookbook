namespace SharedCookbook.Application.Invitations.Commands.DeleteInvitation;

public class DeleteInvitationCommandValidator : AbstractValidator<DeleteInvitationCommand>
{
    public DeleteInvitationCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than zero.");
    }
}
