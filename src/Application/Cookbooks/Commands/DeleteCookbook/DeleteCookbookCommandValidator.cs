namespace SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;

public class DeleteCookbookCommandValidator : AbstractValidator<DeleteCookbookCommand>
{
    public DeleteCookbookCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}
