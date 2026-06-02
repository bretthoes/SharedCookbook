namespace SharedCookbook.Application.Users;

public sealed class DisplayNamePropagation(
    IApplicationDbContext context,
    ILogger<DisplayNamePropagation> logger,
    TimeProvider timeProvider) : IDisplayNamePropagation
{
    public async Task PropagateAsync(string userId, string displayName, CancellationToken ct = default)
    {
        long start = timeProvider.GetTimestamp();

        int membershipsUpdated = await context.CookbookMemberships
            .Where(m => m.CreatedBy == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.DisplayName, displayName), ct);

        long afterMemberships = timeProvider.GetTimestamp();

        int recipesUpdated = await context.Recipes
            .Where(r => r.CreatedBy == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.AuthorDisplayName, displayName), ct);

        long afterRecipes = timeProvider.GetTimestamp();

        logger.LogInformation(
            "DisplayName propagated for {UserId}: {MembershipsUpdated} memberships in {MembershipMs}ms, " +
            "{RecipesUpdated} recipes in {RecipeMs}ms",
            userId,
            membershipsUpdated,
            (long)timeProvider.GetElapsedTime(start, afterMemberships).TotalMilliseconds,
            recipesUpdated,
            (long)timeProvider.GetElapsedTime(afterMemberships, afterRecipes).TotalMilliseconds);
    }
}
