namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public class CreateCookbookCommandValidator : AbstractValidator<CreateCookbookCommand>
{
    public CreateCookbookCommandValidator()
    {
        RuleFor(v => v.Title)
            .MinimumLength(3)
            .MaximumLength(200)
            .NotEmpty();
    }
}
