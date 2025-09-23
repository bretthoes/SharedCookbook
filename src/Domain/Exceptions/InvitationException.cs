namespace SharedCookbook.Domain.Exceptions;

public abstract class InvitationException(string message) : Exception(message);

public sealed class InvitationTokenMismatchException(int invitationId, int tokenId)
    : InvitationException($"Token {tokenId} does not belong to invitation {invitationId}.");

public sealed class InvitationNotPendingException(CookbookInvitationStatus status)
    : InvitationException($"Invitation is not pending (current: {status}).");

public sealed class InvitationTokenInactiveException(InvitationTokenStatus status)
    : InvitationException($"Token is not active (current: {status}).");
