namespace SharedCookbook.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        // TODO this should not allow special characters,
        // should also allow international names
        RuleFor(u => u.DisplayName)
            .MinimumLength(1)
            .MaximumLength(30)
            .NotEmpty();
    }
}
