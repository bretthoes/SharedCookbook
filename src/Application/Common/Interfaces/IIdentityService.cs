using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Users.Queries.GetUser;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<UserDto?> FindByEmailAsync(string email);

    Task<string?> GetUserNameAsync(int? userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, int UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);
}
