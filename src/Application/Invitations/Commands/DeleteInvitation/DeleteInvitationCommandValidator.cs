namespace SharedCookbook.Application.Invitations.Commands.DeleteInvitation;

public class DeleteInvitationCommandValidator : AbstractValidator<DeleteInvitationCommand>
{
    public DeleteInvitationCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}
