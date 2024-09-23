namespace SharedCookbook.Domain.Events;

public class RecipeCreatedEvent : BaseEvent
{
    public RecipeCreatedEvent(Recipe recipe)
    {
        Recipe = recipe;
    }

    public Recipe Recipe { get; }
}
