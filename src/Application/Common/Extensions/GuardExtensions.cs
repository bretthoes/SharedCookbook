using SharedCookbook.Application.Common.Exceptions;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace SharedCookbook.Application.Common.Extensions;

public static class GuardExtensions
{
    public static void TokenDigestMismatch(this IGuardClause _, bool isMatch)
    {
        if (!isMatch) throw new ValidationException(); // TODO throw something more than validation exception apt here
    }
    
    // TODO ensure exception handler knows about this exception type
    public static void InvalidTokenFormat(this IGuardClause guard, bool condition)
    {
        if (!condition) return;

        const string detail = "Invalid token link received.";

        throw new FormatException(detail);
    }
    
    public static void Conflict(this IGuardClause _, bool condition, string reason,
        int cookbookId, string senderId, string recipientId)
    {
        if (!condition) return;

        string detail =
            $"Invite conflict: {reason}. " +
            $"CookbookId={cookbookId}, SenderId={senderId}, RecipientId={recipientId}.";

        throw new ConflictException(detail);
    }

    public static void ExistingMembership(this IGuardClause guard, bool alreadyMember,
        int cookbookId, string senderId, string recipientId)
        => guard.Conflict(alreadyMember, reason: "recipient is already a member",
            cookbookId, senderId, recipientId);

    public static void PendingInvitation(this IGuardClause guard, bool hasPendingInvite,
        int cookbookId, string senderId, string recipientId)
        => guard.Conflict(hasPendingInvite, reason: "an active invitation already exists",
            cookbookId, senderId, recipientId);
}
