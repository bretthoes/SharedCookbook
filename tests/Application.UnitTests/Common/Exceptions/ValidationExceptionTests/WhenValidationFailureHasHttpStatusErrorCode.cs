using System.Net;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using SharedCookbook.Application.Common.Exceptions;

namespace SharedCookbook.Application.UnitTests.Common.Exceptions.ValidationExceptionTests;

public class WhenFileSizeValidationFailure
{
    private ValidationException _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var failures = new List<ValidationFailure>
        {
            new("Files[0].Length", "File size should not exceed 10 MB.")
            {
                ErrorCode = HttpStatusCode.RequestEntityTooLarge.ToString(),
            },
        };

        _actual = new ValidationException(failures);
    }

    [Test]
    public void ThenStatusCodeIs413() =>
        Assert.That(_actual.StatusCode, Is.EqualTo(StatusCodes.Status413PayloadTooLarge));
}

public class WhenUnsupportedMediaTypeValidationFailure
{
    private ValidationException _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var failures = new List<ValidationFailure>
        {
            new("Files[0].FileName", "File must have one of the following extensions: .jpg.")
            {
                ErrorCode = HttpStatusCode.UnsupportedMediaType.ToString(),
            },
        };

        _actual = new ValidationException(failures);
    }

    [Test]
    public void ThenStatusCodeIs415() =>
        Assert.That(_actual.StatusCode, Is.EqualTo(StatusCodes.Status415UnsupportedMediaType));
}
