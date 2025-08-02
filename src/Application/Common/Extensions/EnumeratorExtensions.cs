namespace SharedCookbook.Application.Common.Extensions;

public static class EnumeratorExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> source) =>
        !source.Any();

    public static bool IsNotEmpty<T>(this IEnumerable<T> source) =>
        source.Any();
}
