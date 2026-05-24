using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace SharedCookbook.Infrastructure.Identity;

/// <summary>
/// Extends the default claims principal with a subscription_tier claim so that
/// rate-limiting policies and endpoints can read the user's tier from the JWT
/// without a database round-trip.
/// </summary>
public sealed class SubscriptionClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<ApplicationUser>(userManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim(SubscriptionClaims.Tier, user.SubscriptionTier.ToString()));
        return identity;
    }
}
