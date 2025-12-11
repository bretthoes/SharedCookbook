using FluentValidation.Results;
using SharedCookbook.Application.Common.Exceptions;

namespace SharedCookbook.Application.UnitTests.Common.Exceptions.ValidationExceptionTests;

public class WhenMultipleValidationFailures
{
    private ValidationException _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var failures = new List<ValidationFailure>
        {
            new("Age", "must be 18 or older"),
            new("Age", "must be 25 or younger"),
            new("Password", "must contain at least 8 characters"),
            new("Password", "must contain a digit"),
            new("Password", "must contain upper case letter"),
            new("Password", "must contain lower case letter"),
        };

        _actual = new ValidationException(failures);
    }

    [Test]
    public void ThenCreatesErrorsDictionaryWithMultipleKeys()
    {
        string[] expected = ["Password", "Age"];
        
        Assert.That(_actual.Errors.Keys, Is.EquivalentTo(expected));
    }
    
    [TestCase("Age", new[]{"must be 18 or older", "must be 25 or younger"})]
    [TestCase("Password", new[]
    {
        "must contain at least 8 characters",
        "must contain a digit",
        "must contain upper case letter",
        "must contain lower case letter"
    })]
    public void ThenKeyContainsExpectedValidationErrors(string key, string[] expected)
    {
        Assert.That(_actual.Errors[key], Is.EquivalentTo(expected));;
    }
}

