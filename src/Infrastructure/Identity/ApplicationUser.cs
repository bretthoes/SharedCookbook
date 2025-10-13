using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SharedCookbook.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    [MaxLength(256)]
    public string? DisplayName { get; set; }
}
