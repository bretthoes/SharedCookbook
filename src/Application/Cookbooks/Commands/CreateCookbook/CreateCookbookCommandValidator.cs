namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public class CreateCookbookCommandValidator : AbstractValidator<CreateCookbookCommand>
{
    public CreateCookbookCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
        RuleFor(v => v.ImagePath)
            .NotEmpty();
    }
}
