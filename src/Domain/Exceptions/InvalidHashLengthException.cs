using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.Exceptions;

public sealed class InvalidHashLengthException(int length) 
    : Exception($"Invalid hash length provided: \"{length}\". Expected was: \"{TokenDigest.HashLength}\".");
