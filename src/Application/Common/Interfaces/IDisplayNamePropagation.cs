namespace SharedCookbook.Application.Common.Interfaces;

public interface IDisplayNamePropagation
{
    /// <summary>
    /// Bulk-updates denormalized display name fields on memberships and recipes owned by
    /// <paramref name="userId"/>. Uses direct SQL UPDATE statements so no entities are loaded.
    /// <para>
    /// Intentionally excludes <c>CookbookInvitation.SenderDisplayName</c> and
    /// <c>CookbookNotification.ActorDisplayName/SubjectDisplayName</c>: invitations are
    /// point-in-time records and notifications serve as an immutable activity log, so a
    /// stale display name there is acceptable.
    /// </para>
    /// </summary>
    Task PropagateAsync(string userId, string displayName, CancellationToken ct = default);
}
