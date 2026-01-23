using SharedCookbook.Application.Common.Extensions;

namespace SharedCookbook.Application.UnitTests.Common.Extensions.StringExtensionsTests;

public class IsAbsoluteHttpUrlTests
{
    [TestCase("https://a.com/x.jpg", true)]
    [TestCase("http://a.com/x.jpg", true)]
    [TestCase("//a.com/x.jpg", false)]
    [TestCase("/x.jpg", false)]
    [TestCase("x.jpg", false)]
    public void WhenStringIsAbsoluteHttpUrlReturnsTrue(string input, bool expected)
    {
        bool actual = input.IsAbsoluteHttpUrl();

        Assert.That(actual, Is.EqualTo(expected));
    }
}
