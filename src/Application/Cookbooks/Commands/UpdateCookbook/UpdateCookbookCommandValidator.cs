namespace SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

public class UpdateCookbookCommandValidator : AbstractValidator<UpdateCookbookCommand>
{
    public UpdateCookbookCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
        RuleFor(v => v.ImagePath)
            .NotEmpty();
    }
}
