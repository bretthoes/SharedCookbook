using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public record CreateRecipeCommand : IRequest<int>
{
}

public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateRecipeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
