namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public class CreateCookbookCommandValidator : AbstractValidator<CreateCookbookCommand>
{
    public CreateCookbookCommandValidator()
    {
        RuleFor(v => v.Title)
            .MinimumLength(1)
            .MaximumLength(255)
            .NotEmpty()
            .NotNull();
    }
}
