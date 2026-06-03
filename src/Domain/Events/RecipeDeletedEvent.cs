namespace SharedCookbook.Domain.Events;

public sealed class RecipeDeletedEvent(Guid recipeId) : BaseEvent
{
    public Guid RecipeId { get; } = recipeId;
}
