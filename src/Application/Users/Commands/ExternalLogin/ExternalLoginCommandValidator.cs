namespace SharedCookbook.Application.Users.Commands.ExternalLogin;

public sealed class ExternalLoginCommandValidator : AbstractValidator<ExternalLoginCommand>
{
    public ExternalLoginCommandValidator()
    {
        RuleFor(command => command.Provider)
            .IsInEnum()
            .WithMessage("Provider must be Google, Apple, or Facebook.");

        RuleFor(command => command.Token)
            .NotEmpty();
    }
}
