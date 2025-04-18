namespace SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

public class UpdateCookbookCommandValidator : AbstractValidator<UpdateCookbookCommand>
{
    public UpdateCookbookCommandValidator()
    {
        RuleFor(command => command.Title)
            .MinimumLength(Cookbook.Constraints.TitleMinLength)
            .MaximumLength(Cookbook.Constraints.TitleMaxLength)
            .NotEmpty()
            .NotNull();
    }
}
