namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public sealed class CreateCookbookCommandValidator : AbstractValidator<CreateCookbookCommand>
{
    public CreateCookbookCommandValidator()
    {
        RuleFor(command => command.Title)
            .MinimumLength(Cookbook.Constraints.TitleMinLength)
            .MaximumLength(Cookbook.Constraints.TitleMaxLength)
            .NotEmpty()
            .NotNull();
    }
}
