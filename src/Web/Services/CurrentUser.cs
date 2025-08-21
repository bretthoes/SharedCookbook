using System.Security.Claims;
using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Web.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public string? Id => httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType: ClaimTypes.NameIdentifier);
    public List<string>? Roles => httpContextAccessor.HttpContext?.User?
        .FindAll(type: ClaimTypes.Role)
        .Select(claim => claim.Value)
        .ToList();
}
