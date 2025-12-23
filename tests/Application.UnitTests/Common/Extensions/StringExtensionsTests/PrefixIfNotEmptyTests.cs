using SharedCookbook.Application.Common.Extensions;

namespace SharedCookbook.Application.UnitTests.Common.Extensions;

public class PrefixIfNotEmptyTests
{
    [TestCase(null, "x", "")]
    [TestCase("", "x", "")]
    [TestCase("   ", "x", "")]
    [TestCase("\t\r\n", "x", "")]
    [TestCase("a", "x", "xa")]
    [TestCase(" a ", "x", "x a ")]
    [TestCase("0", "x", "x0")]
    [TestCase("test", "", "test")]
    [TestCase("test", "pre-", "pre-test")]
    [TestCase("test", " ", " test")]
    [TestCase(null!, " ", "")]
    public void ReturnsExpected(string? input, string prefix, string expected)
    {
        var actual = input.PrefixIfNotEmpty(prefix);
        Assert.That(actual, Is.EqualTo(expected));
    }
}
