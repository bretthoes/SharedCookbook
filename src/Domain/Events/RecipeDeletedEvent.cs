namespace SharedCookbook.Domain.Events;

public sealed class RecipeDeletedEvent(int recipeId) : BaseEvent
{
    public int RecipeId { get; } = recipeId;
}
