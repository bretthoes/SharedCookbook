namespace SharedCookbook.Application.RecipeMakes.Commands.RecordRecipeMade;

public sealed record RecordRecipeMadeCommand(Guid RecipeId) : IRequest;

public sealed class RecordRecipeMadeCommandHandler(
    IApplicationDbContext context,
    IUser user)
    : IRequestHandler<RecordRecipeMadeCommand>
{
    public async Task Handle(RecordRecipeMadeCommand request, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);

        var recipe = await context.Recipes.FindOrThrowAsync(request.RecipeId, ct);

        _ = await context.CookbookMemberships.FindForUserAsync(recipe.CookbookId, user.Id, ct)
            ?? throw new ForbiddenAccessException();

        recipe.MadeCount++;

        recipe.AddDomainEvent(new RecipeMadeEvent(recipe.Id, recipe.CookbookId, user.Id));

        await context.SaveChangesAsync(ct);
    }
}
