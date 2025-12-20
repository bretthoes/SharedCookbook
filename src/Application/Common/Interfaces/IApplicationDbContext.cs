namespace SharedCookbook.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Cookbook> Cookbooks { get; }

    DbSet<CookbookInvitation> CookbookInvitations { get; }
    
    DbSet<InvitationToken> InvitationTokens { get; }

    DbSet<CookbookMembership> CookbookMemberships { get; }

    DbSet<Recipe> Recipes { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
