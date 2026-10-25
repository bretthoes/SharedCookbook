using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.Entities;

/// <summary>
/// Share-link invitation. The raw token is not stored; <see cref="Digest"/> is.
/// </summary>
public sealed class InvitationToken : BaseInvitation
{
    private const int TwoWeeksAgo = -14;
    
    public required Guid PublicId { get; init; }
    
    public string? RedeemerPersonId { get; init; }
    
    public TokenDigest Digest { get; init; } = null!;

    /// <summary>
    /// True when the invitation is still active, <see cref="BaseAuditableEntity.Created"/> is less than 14 days ago,
    /// and <c>Created</c> is before the current time. A timestamp at or after now is treated as expired.
    /// </summary>
    public bool IsRedeemable => IsActive && !IsExpired;

    private bool IsExpired => Created <= DateTimeOffset.UtcNow.AddDays(TwoWeeksAgo) || Created >= DateTimeOffset.UtcNow; 
    
    public static InvitationToken IssueNewToken(TokenDigest digest, Guid cookbookId)
    {
        return new InvitationToken
        {
            Status = InvitationStatus.Active,
            Digest = digest,
            CookbookId = cookbookId,
            PublicId = Guid.NewGuid()
        };
    }
}
