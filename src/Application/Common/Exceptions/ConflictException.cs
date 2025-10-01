namespace SharedCookbook.Application.Common.Exceptions;

public class ConflictException(string message) : Exception(message);

public sealed class TokenDigestMismatchException()
    : ConflictException("Token digest mismatch.");

public sealed class TokenIsNotConsumableException()
    : ConflictException("Token is not consumable.");

public sealed class MembershipAlreadyExistsException(int cookbookId, string recipientId)
    : ConflictException(
        $"recipient {recipientId} was invited to cookbook {cookbookId}, but is already a member.");

public sealed class InvitationAlreadyPendingException(int cookbookId, string recipientId)
    : ConflictException(
        $"recipient {recipientId} was invited to cookbook {cookbookId}, but has already been invited.");
