using SharedCookbook.Application.Common.Extensions;

namespace SharedCookbook.Application.UnitTests.Common.Extensions;

public class RemoveHtmlTests
{
    [TestCase("plain text", "plain text")]
    [TestCase("<b>bold</b>", "bold")]
    [TestCase("a<b>b</b>c", "abc")]
    [TestCase("<p>Hello</p><p>World</p>", "HelloWorld")]
    [TestCase("<div>Hello<br/>World</div>", "HelloWorld")]
    [TestCase(" <p> Hello </p> ", "Hello")]
    [TestCase("<span>one</span>    <span>two</span>", "one two")]
    [TestCase("<p>one</p>\n\t<p>two</p>", "one two")]
    [TestCase("<p>one</p>&nbsp;<p>two</p>", "one&nbsp;two")]
    [TestCase("<invalid", "<invalid")]
    [TestCase("", "")]
    public void ReturnsExpected(string input, string expected)
    {
        string actual = input.RemoveHtml();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void RemovesNestedTags()
    {
        const string input = "<div><p>Hello <b>there</b></p></div>";
        Assert.That(input.RemoveHtml(), Is.EqualTo("Hello there"));
    }

    [Test]
    public void RemovesAttributes()
    {
        const string input = "<a href=\"https://example.com\" class=\"x\">link</a>";
        Assert.That(input.RemoveHtml(), Is.EqualTo("link"));
    }
}
