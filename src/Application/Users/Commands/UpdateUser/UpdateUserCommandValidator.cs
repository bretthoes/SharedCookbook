namespace SharedCookbook.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(u => u.DisplayName)
            .NotNull()
            .MaximumLength(255)
            .Matches(@"^[\p{L}\p{M} \-']+$")
            .WithMessage("Display name can only contain letters, spaces, hyphens, and apostrophes.");
    }
}
