using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Recipes.Commands.UpdateRecipe;

public record UpdateRecipeCommand(int Id) : IRequest<int>;

public class UpdateRecipeCommandHandler : IRequestHandler<UpdateRecipeCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateRecipeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
