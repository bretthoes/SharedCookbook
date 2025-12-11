using FluentValidation.Results;
using SharedCookbook.Application.Common.Exceptions;

namespace SharedCookbook.Application.UnitTests.Common.Exceptions.ValidationExceptionTests;

public class WhenMultipleValidationFailures
{
    [Test]
    public void ThenCreatesAMultipleElementErrorDictionaryEachWithMultipleValues()
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

        var actual = new ValidationException(failures).Errors;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Keys, Is.EquivalentTo(["Password", "Age"]));
            Assert.That(actual["Age"], Is.EquivalentTo(["must be 25 or younger", "must be 18 or older"]));
            Assert.That(actual["Password"], Is.EquivalentTo(["must contain lower case letter",
            "must contain upper case letter",
            "must contain at least 8 characters",
            "must contain a digit"
            ]));
        }
    }}
