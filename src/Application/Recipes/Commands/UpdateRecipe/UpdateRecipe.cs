using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Recipes.Commands.UpdateRecipe;

public record UpdateRecipeCommand : IRequest<int>
{
    public int Id { get; init; }
    
    public string? Title { get; init; }
}

public class UpdateRecipeCommandHandler : IRequestHandler<UpdateRecipeCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateRecipeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Recipes
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        if (!string.IsNullOrWhiteSpace(request.Title)) entity.Title = request.Title;

        await _context.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
    }
}
