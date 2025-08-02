namespace SharedCookbook.Domain.Events;

public sealed class RecipeCreatedEvent(Recipe recipe) : BaseEvent
{
    public Recipe Recipe { get; } = recipe;
}
