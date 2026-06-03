namespace SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

public class UpdateCookbookCommandValidator : AbstractValidator<UpdateCookbookCommand>
{
    public UpdateCookbookCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
        RuleFor(command => command.Title)
            .MaximumLength(Cookbook.Constraints.TitleMaxLength)
            .NotEmpty()
            .NotNull();
    }
}
