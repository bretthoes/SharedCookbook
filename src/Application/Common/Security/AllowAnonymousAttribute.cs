namespace SharedCookbook.Application.Common.Security;

/// <summary>
/// Specifies that the class this attribute is applied to does not require authorization.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AllowAnonymousAttribute : Attribute;
