using ValidationException = SharedCookbook.Application.Common.Exceptions.ValidationException;

namespace SharedCookbook.Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            tasks: validators.Select(validator =>
                validator.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(result => result.Errors.Count != 0)
            .SelectMany(result => result.Errors)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
