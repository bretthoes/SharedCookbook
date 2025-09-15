using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Infrastructure.TokenService;

public sealed class InvitationTokenService : IInvitationTokenService
{
    private const int CodeBytes = 24;
    private const int SaltBytes = 16;

    public GeneratedLinkCode GenerateLinkCode()
    {
        var code = RandomNumberGenerator.GetBytes(CodeBytes);
        var codeToken = WebEncoders.Base64UrlEncode(code);

        var salt = RandomNumberGenerator.GetBytes(SaltBytes);
        var material = new byte[salt.Length + code.Length];
        Buffer.BlockCopy(salt, 0, material, 0, salt.Length);
        Buffer.BlockCopy(code, 0, material, salt.Length, code.Length);

        var hash = SHA256.HashData(material);
        return new GeneratedLinkCode(codeToken, new HashedInvitationCode(hash, salt));
    }

    public bool Verify(string codeToken, HashedInvitationCode stored)
    {
        byte[] code;
        try { code = WebEncoders.Base64UrlDecode(codeToken); }
        catch { return false; }

        var material = new byte[stored.Salt.Length + code.Length];
        Buffer.BlockCopy(stored.Salt, 0, material, 0, stored.Salt.Length);
        Buffer.BlockCopy(code, 0, material, stored.Salt.Length, code.Length);

        var computed = SHA256.HashData(material);
        return stored.Hash.Length == computed.Length &&
               CryptographicOperations.FixedTimeEquals(stored.Hash, computed);
    }

    public InvitationToken? Parse(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        int dot = raw.IndexOf('.');
        if (dot <= 0)
        {
            return int.TryParse(raw, out int onlyId) ? new InvitationToken.Email(onlyId) : null;
        }

        if (!int.TryParse(raw[..dot], out int id)) return null;
        var code = raw[(dot + 1)..];
        return string.IsNullOrEmpty(code) ? null : new InvitationToken.Link(id, code);
    }
}
