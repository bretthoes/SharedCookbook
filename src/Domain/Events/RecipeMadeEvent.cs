namespace SharedCookbook.Domain.Events;

public sealed class RecipeMadeEvent(Guid recipeId, Guid cookbookId, string actorUserId) : BaseEvent
{
    public Guid RecipeId { get; } = recipeId;

    public Guid CookbookId { get; } = cookbookId;

    public string ActorUserId { get; } = actorUserId;
}
