using System.Reflection;

namespace SharedCookbook.Web.Infrastructure;

public static class MethodInfoExtensions
{
    internal static bool IsAnonymous(this MethodInfo method)
    {
        char[] invalidChars = ['<', '>'];
        return method.Name.Any(invalidChars.Contains);
    }
}
