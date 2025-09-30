using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.UpdateInvitation;

public class UpdateInvitationCommandValidator : AbstractValidator<UpdateInvitationCommand>
{
    public UpdateInvitationCommandValidator()
    {
        RuleFor(command => command.NewStatus)
            .IsInEnum()
            .WithMessage($"NewStatus must be a valid status: 1=Active, 2=Accepted, 3=Rejected, 4=Revoked.")
            .Must(status => Enum.IsDefined(typeof(InvitationStatus), status))
            .WithMessage("Invalid status. Must be a valid InvitationStatus.");
    }
}
