using System.Net;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace SharedCookbook.Application.Common.Exceptions;

public class ValidationException() : Exception("One or more validation failures have occurred.")
{
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        var failureList = failures.ToList();
        Errors = failureList
            .GroupBy(failure => failure.PropertyName, failure => failure.ErrorMessage)
            .ToDictionary(failureGroup
                => failureGroup.Key, failureGroup => failureGroup.ToArray());
        StatusCode = ResolveStatusCode(failureList);
    }

    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

    public int StatusCode { get; } = StatusCodes.Status400BadRequest;

    private static int ResolveStatusCode(IReadOnlyList<ValidationFailure> failures)
    {
        if (failures.Any(f => HasErrorCode(f, HttpStatusCode.RequestEntityTooLarge)))
            return StatusCodes.Status413PayloadTooLarge;

        if (failures.Any(f => HasErrorCode(f, HttpStatusCode.UnsupportedMediaType)))
            return StatusCodes.Status415UnsupportedMediaType;

        return StatusCodes.Status400BadRequest;
    }

    private static bool HasErrorCode(ValidationFailure failure, HttpStatusCode statusCode) =>
        string.Equals(failure.ErrorCode, statusCode.ToString(), StringComparison.Ordinal);
}
