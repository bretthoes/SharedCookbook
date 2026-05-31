namespace SharedCookbook.Application.Contracts;

public sealed record NotificationDto
{
    public required int Id { get; init; }

    public required CookbookNotificationActionType ActionType { get; init; }

    public required DateTimeOffset Created { get; init; }

    public required int CookbookId { get; init; }

    public required string CookbookTitle { get; init; }

    public int? RecipeId { get; init; }

    public string? RecipeTitle { get; init; }

    public string? ActorDisplayName { get; init; }

    public string? SubjectDisplayName { get; init; }

    public bool IsImportant { get; init; }
}
