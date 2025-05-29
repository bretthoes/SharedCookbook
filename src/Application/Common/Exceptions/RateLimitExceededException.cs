namespace SharedCookbook.Application.Common.Exceptions;

public class RateLimitExceededException() : Exception("Too many requests. Please try again later.");
