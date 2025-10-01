using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Infrastructure.Security;

public sealed class Sha256TokenFactory : IInvitationTokenFactory
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
        return new MintedToken(inviteToken, new TokenDigest(hash, salt));
    }

    public bool Verify(string secret, TokenDigest stored)
    {
        byte[] code;
        try { code = WebEncoders.Base64UrlDecode(secret); }
        catch { return false; }

        var material = new byte[stored.Salt.Length + code.Length];
        Buffer.BlockCopy(stored.Salt, 0, material, 0, stored.Salt.Length);
        Buffer.BlockCopy(code, 0, material, stored.Salt.Length, code.Length);

        byte[] computed = SHA256.HashData(material);
        return stored.Hash.Length == computed.Length &&
               CryptographicOperations.FixedTimeEquals(left: stored.Hash, right: computed);
    }
}
