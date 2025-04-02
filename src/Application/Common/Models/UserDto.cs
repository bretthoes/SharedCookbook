namespace SharedCookbook.Application.Common.Models;

// TODO remove this; just use ApplicationUser
public class UserDto
{
    public required string Id { get; init; }
    public required string Email { get; init; }
    public string? UserName { get; init; }
    public string? DisplayName { get; init; }
}
