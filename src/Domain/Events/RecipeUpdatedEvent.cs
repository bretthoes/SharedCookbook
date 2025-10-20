namespace SharedCookbook.Domain.Events;

public sealed class RecipeUpdatedEvent(int recipeId) : BaseEvent
{
    public int RecipeId { get; } = recipeId;
}
