using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.UpdateInvitation;

public class UpdateInvitationCommandValidator : AbstractValidator<UpdateInvitationCommand>
{
    public UpdateInvitationCommandValidator()
    {
        RuleFor(command => command.NewStatus)
            .IsInEnum()
            .WithMessage("NewStatus must be a valid status: 0=Unknown, 1=Sent, 2=Accepted, or 3=Rejected.")
            .Must(status => Enum.IsDefined(typeof(CookbookInvitationStatus), status))
            .WithMessage("Invalid status. Must be a valid CookbookInvitationStatus.");
    }
}
