namespace SharedCookbook.Application.Common.Models;

public class UserDto
{
    public required string Id { get; init; }
    public required string Email { get; init; }
    public string? UserName { get; init; }
}
