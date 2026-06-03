namespace SharedCookbook.Domain.Events;

public sealed class RecipeUpdatedEvent(Guid recipeId) : BaseEvent
{
    public Guid RecipeId { get; } = recipeId;
}
