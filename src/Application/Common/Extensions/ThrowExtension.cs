namespace SharedCookbook.Application.Common.Extensions;

public static class Throw
{
    public static void IfFalse<T>(bool condition)
        where T : Exception, new()
    {
        if (condition) return;
        
        throw new T();
    }
}
