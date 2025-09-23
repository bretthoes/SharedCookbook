using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.Exceptions;

public class InvalidSaltLengthException(int length) 
    : Exception($"Invalid salt length provided: \"{length}\". Expected was: \"{TokenDigest.SaltLength}\".");
