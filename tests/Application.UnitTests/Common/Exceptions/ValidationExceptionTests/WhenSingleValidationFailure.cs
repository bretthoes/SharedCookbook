using FluentValidation.Results;
using SharedCookbook.Application.Common.Exceptions;

namespace SharedCookbook.Application.UnitTests.Common.Exceptions.ValidationExceptionTests;

public class WhenSingleValidationFailure
{
    private ValidationException _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var failures = new List<ValidationFailure>
        {
            new("Age", "must be over 18"),
        };

        _actual = new ValidationException(failures);
    }
    
    [Test]
    public void ThenCreatesAnErrorDictionaryWithASingleKey()
    {
        string[] expected = ["Age"];
        
        Assert.That(_actual.Errors.Keys, Is.EqualTo(expected));
    }
    
    [Test]
    public void ThenCreatesAnErrorDictionaryWithASingleValueForTheKey()
    {
        string[] expected = ["must be over 18"];
        
        Assert.That(_actual.Errors["Age"], Is.EquivalentTo(expected));
    }
}
