namespace SharedCookbook.Application.Common.Exceptions;

public class ConflictException(string message) : Exception(message);

public sealed class TokenDigestMismatchException()
    : ConflictException("Token digest mismatch.");

public sealed class TokenIsNotConsumableException()
    : ConflictException("Token is not consumable.");
