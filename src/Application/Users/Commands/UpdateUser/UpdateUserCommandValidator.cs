namespace SharedCookbook.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(command => command.DisplayName)
            .NotEmpty()
            .WithMessage("Display name can't be blank.")
            .MaximumLength(256)
            .Matches(expression: @"^[\p{L}\p{M} \-']+$")
            .WithMessage("Display name can only contain letters, spaces, hyphens, and apostrophes.");
    }
}
