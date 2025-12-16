namespace SharedCookbook.Application.Contracts;

public sealed record RecipeBriefDto
{
    public required int Id { get; init; }

    public required string Title { get; init; }
}

public static partial class Extensions
{
    private static RecipeBriefDto ToDto(this Recipe recipe) => new() { Id = recipe.Id, Title = recipe.Title };
    
    public static IQueryable<RecipeBriefDto> ToDtos(this IQueryable<Recipe> query) =>
        query.Select(recipe => recipe.ToDto());

}
