using System.Security.Claims;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Web.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public string? Id => httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType: ClaimTypes.NameIdentifier);
    public List<string>? Roles => httpContextAccessor.HttpContext?.User?
        .FindAll(type: ClaimTypes.Role)
        .Select(claim => claim.Value)
        .ToList();

    public bool IsPro
    {
        get
        {
            var tier = httpContextAccessor.HttpContext?.User?.FindFirstValue(SubscriptionClaims.Tier)
                       ?? nameof(SubscriptionTier.Free);
            return tier == nameof(SubscriptionTier.Pro);
        }
    }
}
