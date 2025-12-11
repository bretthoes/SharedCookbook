using SharedCookbook.Application.Common.Exceptions;

namespace SharedCookbook.Application.UnitTests.Common.Exceptions.ValidationExceptionTests;

public class WhenDefaultConstructor
{
    [Test]
    public void ThenCreatesAnEmptyErrorDictionary()
    {
        string[] expected = [];
        
        var actual = new ValidationException().Errors;

        Assert.That(actual.Keys, Is.EquivalentTo(expected));
    }
}
