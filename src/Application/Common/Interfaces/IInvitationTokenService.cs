namespace SharedCookbook.Application.Common.Interfaces;

public interface IInvitationTokenService
{
    /// <summary>
    /// Mints a new invitation token.
    /// </summary>
    /// <returns>
    /// A <see cref="MintedToken"/> containing:
    /// <list type="bullet">
    /// <item><description><c>InviteToken</c> — Base64URL-encoded secret to send to the client.</description></item>
    /// <item><description><c>Stored</c> — <see cref="HashedToken"/> with <c>Hash</c> (SHA-256 of salt||code) and <c>Salt</c> to persist in the database.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// Store only the hash and salt. Never persists the raw code. Build the deep link as
    /// <c>{invitationId}.{InviteToken}</c> on the client or controller.
    /// </remarks>
    MintedToken Mint();
    
    /// <summary>
    /// Verifies a Base64URL-encoded invitation <paramref name="inviteToken"/> against a stored hash and salt.
    /// </summary>
    /// <param name="inviteToken">The Base64URL-encoded token received from the client (no padding).</param>
    /// <param name="stored">The stored <see cref="HashedToken"/> (hash and salt) from the invitation row.</param>
    /// <returns><c>true</c> if the token matches the stored hash; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// This method performs only cryptographic verification. It does not enforce TTL, status, or authorization.
    /// Callers must check invitation state (e.g., <c>Sent</c>), and expiry derived from <c>Created</c>.
    /// </remarks>
    bool Verify(string inviteToken, HashedToken stored);
    
    /// <summary>
    /// Parses a raw invitation token of the form <c>{invitationId}.{inviteToken}</c>.
    /// </summary>
    /// <param name="raw">The raw token string taken from the deep link or request.</param>
    /// <returns>
    /// An <see cref="InvitationTokenReference"/> with the numeric invitation id and the Base64URL token,
    /// or <c>null</c> when the format is invalid.
    /// </returns>
    /// <remarks>
    /// This method does not validate the code's Base64URL content. Use <see cref="Verify"/> to validate cryptographically.
    /// </remarks>
    InvitationTokenReference? Parse(string raw);
}

public sealed record HashedToken(byte[] Hash, byte[] Salt);
public sealed record MintedToken(string InviteToken, HashedToken Hashed);
public sealed record InvitationTokenReference(int InvitationId, string InviteToken);


