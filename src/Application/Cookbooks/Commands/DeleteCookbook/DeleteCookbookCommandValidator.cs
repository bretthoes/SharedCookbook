namespace SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;

public class DeleteCookbookCommandValidator : AbstractValidator<DeleteCookbookCommand>
{
    public DeleteCookbookCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than zero.");
    }
}
