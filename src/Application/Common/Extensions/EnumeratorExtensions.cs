namespace SharedCookbook.Application.Common.Extensions;

public static class EnumeratorExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        public bool IsEmpty() => !source.Any();
        public bool IsNotEmpty() => source.Any();
    }
}
