using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Infrastructure.Security;

public sealed class InvitationTokenService : IInvitationTokenService
{
    private const int CodeBytes = 24;
    private const int SaltBytes = 16;

    public MintedToken Mint()
    {
        byte[] code = RandomNumberGenerator.GetBytes(count: CodeBytes);
        string inviteToken = WebEncoders.Base64UrlEncode(code);

        byte[] salt = RandomNumberGenerator.GetBytes(count: SaltBytes);
        var material = new byte[salt.Length + code.Length];
        Buffer.BlockCopy(salt, 0, material, 0, salt.Length);
        Buffer.BlockCopy(code, 0, material, salt.Length, code.Length);

        byte[] hash = SHA256.HashData(material);
        return new MintedToken(inviteToken, new HashedToken(hash, salt));
    }

    public bool Verify(string inviteToken, HashedToken stored)
    {
        byte[] code;
        try { code = WebEncoders.Base64UrlDecode(inviteToken); }
        catch { return false; }

        var material = new byte[stored.Salt.Length + code.Length];
        Buffer.BlockCopy(stored.Salt, 0, material, 0, stored.Salt.Length);
        Buffer.BlockCopy(code, 0, material, stored.Salt.Length, code.Length);

        byte[] computed = SHA256.HashData(material);
        return stored.Hash.Length == computed.Length &&
               CryptographicOperations.FixedTimeEquals(left: stored.Hash, right: computed);
    }

    public InvitationTokenReference? Parse(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        int dot = raw.IndexOf('.');

        if (!int.TryParse(raw[..dot], out int id)) return null;
        string code = raw[(dot + 1)..];
        return string.IsNullOrEmpty(code) ? null : new InvitationTokenReference(id, code);
    }
}
