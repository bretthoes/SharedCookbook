namespace SharedCookbook.Application.InvitationTokens.Commands.UpdateInvitationToken;

public class UpdateInvitationTokenCommandValidator : AbstractValidator<UpdateInvitationTokenCommand>
{
    public UpdateInvitationTokenCommandValidator()
    {
        RuleFor(command => command.Token)
            .NotEmpty().WithMessage("Token cannot be empty.");
    }
}
