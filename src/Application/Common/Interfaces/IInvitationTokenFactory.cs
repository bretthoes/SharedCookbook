using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IInvitationTokenFactory
{
    /// <summary>
    /// Mints a new invitation token.
    /// </summary>
    /// <returns>
    /// A <see cref="MintedToken"/> containing:
    /// <list type="bullet">
    /// <item><description><c>InviteToken</c> — Base64URL-encoded secret to send to the client.</description></item>
    /// <item><description><c>Stored</c> — <see cref="TokenDigest"/> with <c>Hash</c> (SHA-256 of salt||code) and <c>Salt</c> to persist in the database.</description></item>
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
    /// <param name="stored">The stored <see cref="TokenDigest"/> (hash and salt) from the invitation row.</param>
    /// <returns><c>true</c> if the token matches the stored hash; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// This method performs only cryptographic verification. It does not enforce TTL, status, or authorization.
    /// Callers must check invitation state (e.g., <c>Sent</c>), and expiry derived from <c>Created</c>.
    /// </remarks>
    bool Verify(string inviteToken, TokenDigest stored);
}

public sealed record MintedToken(string InviteToken, TokenDigest HashDetails);

public readonly record struct TokenLink(int TokenId, string Secret)
{
    // "<tokenId>.<secret>"
    public override string ToString() => $"{TokenId}.{Secret}";

    public static bool TryParse(string? raw, out TokenLink link)
    {
        link = default;
        if (string.IsNullOrWhiteSpace(raw)) return false;
        int dot = raw.IndexOf('.');
        if (dot <= 0 || dot == raw.Length - 1) return false;
        if (!int.TryParse(s: raw.AsSpan(start: 0, length: dot), out int id)) return false;
        link = new TokenLink(id, raw[(dot + 1)..]);
        return true;
    }
}


