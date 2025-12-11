using FluentValidation.Results;
using SharedCookbook.Application.Common.Exceptions;

namespace SharedCookbook.Application.UnitTests.Common.Exceptions.ValidationExceptionTests;

public class WhenSingleValidationFailure
{
    [Test]
    public void SingleValidationFailureCreatesASingleElementErrorDictionary()
    {
        var failures = new List<ValidationFailure>
        {
            new("Age", "must be over 18"),
        };

        var actual = new ValidationException(failures).Errors;

        Assert.That(actual.Keys, Is.EqualTo(["Age"]));
        Assert.That(actual["Age"], Is.EquivalentTo(["must be over 18"]));
    }
}
