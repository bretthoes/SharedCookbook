using System.Diagnostics.CodeAnalysis;

namespace SharedCookbook.Web.Infrastructure;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapGet(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern = "")
        => builder.MapGet(pattern, handler).WithName(NameOrThrow(handler));

    public static RouteHandlerBuilder MapPost(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern = "")
        => builder.MapPost(pattern, handler).WithName(NameOrThrow(handler));

    public static RouteHandlerBuilder MapPut(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern = "")
        => builder.MapPut(pattern, handler).WithName(NameOrThrow(handler));

    public static RouteHandlerBuilder MapDelete(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern = "")
        => builder.MapDelete(pattern, handler).WithName(NameOrThrow(handler));

    private static string NameOrThrow(Delegate handler)
    {
        var methodInfo = handler.Method;
        if (methodInfo.IsAnonymous())
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.",
                nameof(handler));
        return methodInfo.Name;
    }
}
