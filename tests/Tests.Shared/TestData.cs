namespace SharedCookbook.Tests.Shared;

public static class TestData
{
    public const string AnyNonEmptyString = "value";

    public const char AnyNonEmptyChar = 'x';

    public const string AnyEmail = "test@example.com";
    
    public const string AnyBaseUrl = "https://example.com";
    
    public const string AnyImageFile = "test.jpg";

    public const int AnyPositiveInt = 1;

    public static Guid AnyGuid() => Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static DateTimeOffset AnyPastDate() =>
        new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset AnyFutureDate() =>
        new(3025, 1, 1, 0, 0, 0, TimeSpan.Zero);
}
