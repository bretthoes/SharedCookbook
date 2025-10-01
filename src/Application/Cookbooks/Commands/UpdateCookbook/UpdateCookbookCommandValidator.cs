namespace SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

public class UpdateCookbookCommandValidator : AbstractValidator<UpdateCookbookCommand>
{
    public UpdateCookbookCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than zero.");
        RuleFor(command => command.Title)
            .MinimumLength(Cookbook.Constraints.TitleMinLength)
            .MaximumLength(Cookbook.Constraints.TitleMaxLength)
            .NotEmpty()
            .NotNull();
    }
}
