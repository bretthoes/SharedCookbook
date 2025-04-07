namespace SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

public class UpdateCookbookCommandValidator : AbstractValidator<UpdateCookbookCommand>
{
    public UpdateCookbookCommandValidator()
    {
        RuleFor(command => command.Title)
            .MinimumLength(1)
            .MaximumLength(255)
            .NotEmpty()
            .NotNull();
    }
}
