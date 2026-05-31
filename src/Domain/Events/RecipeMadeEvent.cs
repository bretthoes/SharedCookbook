namespace SharedCookbook.Domain.Events;

public sealed class RecipeMadeEvent(int recipeId, int cookbookId, string actorUserId) : BaseEvent
{
    public int RecipeId { get; } = recipeId;

    public int CookbookId { get; } = cookbookId;

    public string ActorUserId { get; } = actorUserId;
}
