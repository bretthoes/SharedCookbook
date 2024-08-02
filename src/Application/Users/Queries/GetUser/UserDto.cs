namespace SharedCookbook.Application.Users.Queries.GetUser;

public class UserDto
{
    public required int Id { get; set; }
    public required string Email { get; set; }
    public string? UserName { get; set; }
}
