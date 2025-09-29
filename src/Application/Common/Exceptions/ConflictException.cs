namespace SharedCookbook.Application.Common.Exceptions;

public class ConflictException(string message) : Exception(message);

public sealed class TokenDigestMismatchException()
    : ConflictException("Digest mismatch.");
