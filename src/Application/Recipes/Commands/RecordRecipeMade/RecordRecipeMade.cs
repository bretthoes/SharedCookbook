using SharedCookbook.Application.Memberships.Queries.FindForUser;

namespace SharedCookbook.Application.Recipes.Commands.RecordRecipeMade;

public sealed record RecordRecipeMadeCommand(Guid RecipeId) : IRequest;

public sealed class RecordRecipeMadeCommandHandler(
    IApplicationDbContext context,
    IUser user)
    : IRequestHandler<RecordRecipeMadeCommand>
{
    public async Task Handle(RecordRecipeMadeCommand request, CancellationToken ct = default)
    {
        var userId = user.Id;
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var recipe = await context.Recipes.FindOrThrowAsync(request.RecipeId, ct);

        _ = await context.CookbookMemberships.FindForUserAsync(recipe.CookbookId, userId, ct)
            ?? throw new ForbiddenAccessException();

        recipe.RecordMade(userId);

        await context.SaveChangesAsync(ct);
    }
}
