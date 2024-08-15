using System.Security.Claims;

using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Web.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? Id
    {
        get
        {
            var idString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(idString, out var id) 
                ? id 
                : null;
        }
    }
}
