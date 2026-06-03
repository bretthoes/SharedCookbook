namespace SharedCookbook.Application.Contracts;

public sealed record NotificationDto
{
    public required Guid Id { get; init; }

    public required CookbookNotificationActionType ActionType { get; init; }

    public required DateTimeOffset Created { get; init; }

    public required Guid CookbookId { get; init; }

    public required string CookbookTitle { get; init; }

    public Guid? RecipeId { get; init; }

    public string? RecipeTitle { get; init; }

    public string? ActorDisplayName { get; init; }

    public string? SubjectDisplayName { get; init; }

    public bool IsImportant { get; init; }
}
