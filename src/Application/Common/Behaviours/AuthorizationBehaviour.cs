using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Common.Security;

namespace SharedCookbook.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse>(
    IUser user,
    IIdentityService identityService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = request
            .GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToArray();

        // Must be authenticated user
        if (user.Id == null || user.Id == Guid.Empty.ToString())
            throw new UnauthorizedAccessException();
        
        if (authorizeAttributes.IsEmpty())
            return await next(cancellationToken);

        // Role-based authorization
        var authorizeAttributesWithRoles = authorizeAttributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
            .ToArray();

        if (authorizeAttributesWithRoles.IsNotEmpty())
        {
            var authorized = false;

            foreach (string[] roles in authorizeAttributesWithRoles.Select(attribute => attribute.Roles.Split(',')))
            {
                if (roles.Select(role => user.Roles?.Any(userRole => role == userRole) ?? false).Any(isInRole => isInRole))
                    authorized = true;
            }

            // Must be a member of at least one role in roles
            if (!authorized)
                throw new ForbiddenAccessException();
        }

        // Policy-based authorization
        var authorizeAttributesWithPolicies = authorizeAttributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Policy))
            .ToArray();
        
        if (authorizeAttributesWithPolicies.IsEmpty())
            return await next(cancellationToken);

        {
            foreach (string policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
            {
                bool authorized = await identityService.AuthorizeAsync(user.Id, policy);

                if (!authorized)
                    throw new ForbiddenAccessException();
            }
        }

        // User is authorized / authorization not required
        return await next(cancellationToken);
    }
}
