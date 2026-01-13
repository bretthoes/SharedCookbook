using SharedCookbook.Application.Common.Extensions;

namespace SharedCookbook.Application.UnitTests.Common.Extensions.StringExtensionsTests;

public class TruncateTests
{
    [TestCase("", 0, "")]
    [TestCase("", 5, "")]
    [TestCase("a", 0, "")]
    [TestCase("a", 1, "a")]
    [TestCase("abc", 2, "ab")]
    [TestCase("abc", 3, "abc")]
    [TestCase("abc", 10, "abc")]
    [TestCase("hello world", 5, "hello")]
    public void ReturnsExpected(string input, int maxLength, string expected)
    {
        string actual = input.Truncate(maxLength);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ThrowsWhenMaxLengthIsNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = string.Empty.Truncate(-1));
    }
}
