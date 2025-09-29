namespace SharedCookbook.Application.Common.Exceptions;

public sealed class TokenDigestMismatchException(string? detail = null)
    : ConflictException(detail ?? "Digest mismatch")
{
    public static void Throw(string? detail = null)
        => throw new TokenDigestMismatchException(detail);

    public static void ThrowIfFalse(bool condition, string? detail = null)
    {
        if (!condition) throw new TokenDigestMismatchException(detail);
    }
}

public sealed class InvalidTokenStatusException(string? detail = null)
    : ConflictException(detail ?? "This invite is not active.")
{
    public static void Throw(string? detail = null)
        => throw new InvalidTokenStatusException(detail);

    public static void ThrowIfFalse(bool condition, string? detail = null)
    {
        if (!condition) throw new InvalidTokenStatusException(detail);
    }
}
