namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public class CreateCookbookCommandValidator : AbstractValidator<CreateCookbookCommand>
{
    public CreateCookbookCommandValidator()
    {
        RuleFor(v => v.Title)
            .MinimumLength(Cookbook.Constraints.TitleMinLength)
            .MaximumLength(Cookbook.Constraints.TitleMaxLength)
            .NotEmpty()
            .NotNull();
    }
}
