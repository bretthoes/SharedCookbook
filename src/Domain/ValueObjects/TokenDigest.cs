namespace SharedCookbook.Domain.ValueObjects;

public sealed class TokenDigest : ValueObject
{
    public const int HashLength = 32; // SHA-256
    public const int SaltLength = 16; // 128-bit salt

    public byte[] Hash { get; }
    public byte[] Salt { get; }

    public TokenDigest(byte[] hash, byte[] salt)
    {
        if (hash.Length != HashLength) throw new InvalidHashLengthException(hash.Length);
        if (salt.Length != SaltLength) throw new InvalidSaltLengthException(salt.Length);
        Hash = (byte[])hash.Clone();
        Salt = (byte[])salt.Clone();
    }

    private bool Equals(TokenDigest? other) =>
        other is not null &&
        Hash.AsSpan().SequenceEqual(other.Hash) &&
        Salt.AsSpan().SequenceEqual(other.Salt);

    public override bool Equals(object? o) => Equals(o as TokenDigest);
    public override int GetHashCode() => HashCode.Combine(
        HashCode.Combine(Hash.Length, Hash[0], Hash[^1]),
        HashCode.Combine(Salt.Length, Salt[0], Salt[^1]));
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
        yield return Salt;
    }
}
