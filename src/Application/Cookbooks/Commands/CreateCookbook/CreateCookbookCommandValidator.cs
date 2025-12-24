namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public sealed class CreateCookbookCommandValidator : AbstractValidator<CreateCookbookCommand>
{
    public CreateCookbookCommandValidator()
    {
        RuleFor(command => command.Title)
            .MaximumLength(Cookbook.Constraints.TitleMaxLength)
            .NotEmpty()
            .NotNull();
    }
}
