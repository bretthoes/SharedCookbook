namespace SharedCookbook.Application.Users.Queries.GetUser;

public class UserDto
{
    public required int Id { get; init; }
    public required string Email { get; init; }
    public string? UserName { get; init; }
}
