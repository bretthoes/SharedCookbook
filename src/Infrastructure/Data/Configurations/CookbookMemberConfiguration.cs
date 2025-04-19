using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class CookbookMemberConfiguration : IEntityTypeConfiguration<CookbookMembership>
{
    public void Configure(EntityTypeBuilder<CookbookMembership> builder)
    {
        builder.ToTable("cookbook_member");

        builder.HasKey(membership => membership.Id)
            .HasName("PK_cookbook_member_id");

        builder.HasIndex(
            membership => membership.CookbookId, 
            name: "IX_cookbook_member__cookbook_id");
        builder.HasIndex(
            membership => membership.CreatedBy, 
            name: "IX_cookbook_member__created_by");

        builder.Property(membership => membership.Id)
            .HasColumnName("cookbook_member_id")
            .IsRequired();
        builder.Property(membership => membership.CookbookId)
            .HasColumnName("cookbook_id")
            .IsRequired();
        builder.Property(membership => membership.IsCreator)
            .HasColumnName("is_creator")
            .IsRequired();
        builder.Property(membership => membership.CanAddRecipe)
            .HasColumnName("can_add_recipe")
            .IsRequired();
        builder.Property(membership => membership.CanUpdateRecipe)
            .HasColumnName("can_update_recipe")
            .IsRequired();
        builder.Property(membership => membership.CanDeleteRecipe)
            .HasColumnName("can_delete_recipe")
            .IsRequired();
        builder.Property(membership => membership.CanSendInvite)
            .HasColumnName("can_send_invite")
            .IsRequired();
        builder.Property(membership => membership.CanRemoveMember)
            .HasColumnName("can_remove_member")
            .IsRequired();
        builder.Property(membership => membership.CanEditCookbookDetails)
            .HasColumnName("can_edit_cookbook_details")
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany(user => user.CookbookMemberships)
            .HasForeignKey(membership => membership.CreatedBy)
            .HasConstraintName("FK_cookbook_member__created_by")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.HasOne(membership => membership.Cookbook)
            .WithMany(cookbook => cookbook.Memberships)
            .HasForeignKey(membership => membership.CookbookId)
            .HasConstraintName("FK_cookbook_member__cookbook_id")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
