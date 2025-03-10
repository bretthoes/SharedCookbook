using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class CookbookConfiguration : IEntityTypeConfiguration<Cookbook>
{
    public void Configure(EntityTypeBuilder<Cookbook> builder) 
    {
        builder.ToTable("cookbook");

        builder.HasKey(cookbook => cookbook.Id)
            .HasName("PK_cookbook_id");

        builder.HasIndex(
            cookbook => cookbook.CreatedBy, 
            name: "IX_cookbook_creator__created_by");

        builder.Property(cookbook => cookbook.Id)
            .HasColumnName("cookbook_id")
            .IsRequired();
        builder.Property(cookbook => cookbook.Title)
            .HasColumnName("title")
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(cookbook => cookbook.Image)
            .HasColumnName("image")
            .HasMaxLength(255);

        builder.HasOne<Identity.ApplicationUser>()
            .WithMany(user => user.Cookbooks)
            .HasForeignKey(cookbook => cookbook.CreatedBy)
            .HasConstraintName("FK_cookbook__created_by");
        builder.HasMany(cookbook => cookbook.CookbookInvitations)
            .WithOne(invitation => invitation.Cookbook)
            .HasForeignKey(invitation => invitation.CookbookId);
        builder.HasMany(cookbook => cookbook.CookbookMembers)
            .WithOne(member => member.Cookbook)
            .HasForeignKey(member => member.CookbookId);
        builder.HasMany(cookbook => cookbook.CookbookNotifications)
            .WithOne(notification => notification.Cookbook)
            .HasForeignKey(notification => notification.CookbookId);
        builder.HasMany(cookbook => cookbook.Recipes)
            .WithOne(recipe => recipe.Cookbook)
            .HasForeignKey(recipe => recipe.CookbookId);
    }
}
