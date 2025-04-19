using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    [MaxLength(256)]
    public string? DisplayName { get; set; }
    
    public virtual ICollection<Cookbook> Cookbooks { get; set; } = [];

    public virtual ICollection<CookbookMembership> CookbookMemberships { get; set; } = [];

    public virtual ICollection<CookbookNotification> CookbookNotifications { get; set; } = [];

    public virtual ICollection<CookbookInvitation> SentInvitations { get; set; } = [];

    public virtual ICollection<CookbookInvitation> ReceivedInvitations { get; set; } = [];

    public virtual ICollection<Recipe> Recipes { get; set; } = [];
}
