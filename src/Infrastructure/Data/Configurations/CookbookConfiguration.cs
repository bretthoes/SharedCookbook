using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class CookbookConfiguration : IEntityTypeConfiguration<Cookbook>
{
    public void Configure(EntityTypeBuilder<Cookbook> builder) 
    {
        builder.ToTable("cookbook");

        builder.HasKey(c => c.Id)
            .HasName("PK_cookbook_id");

        builder.HasIndex(
            c => c.CreatorPersonId, 
            "IX_cookbook_creator__person_id");

        builder.Property(c => c.Id)
            .HasColumnName("cookbook_id")
            .IsRequired();
        builder.Property(c => c.CreatorPersonId)
            .HasColumnName("creator_person_id");
        builder.Property(c => c.Title)
            .HasColumnName("title")
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(c => c.Image)
            .HasColumnName("image")
            .HasMaxLength(255);

        builder.HasOne<Identity.ApplicationUser>()
            .WithMany(p => p.Cookbooks)
            .HasForeignKey(c => c.CreatorPersonId)
            .HasConstraintName("FK_cookbook__creator_person_id");
        builder.HasMany(c => c.CookbookInvitations)
            .WithOne(ci => ci.Cookbook)
            .HasForeignKey(ci => ci.CookbookId);
        builder.HasMany(c => c.CookbookMembers)
            .WithOne(cm => cm.Cookbook)
            .HasForeignKey(cm => cm.CookbookId);
        builder.HasMany(c => c.CookbookNotifications)
            .WithOne(cn => cn.Cookbook)
            .HasForeignKey(cn => cn.CookbookId);
        builder.HasMany(c => c.Recipes)
            .WithOne(r => r.Cookbook)
            .HasForeignKey(r => r.CookbookId);
    }
}
