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

        builder.HasKey(member => member.Id)
            .HasName("PK_cookbook_member_id");

        builder.HasIndex(
            member => member.CookbookId, 
            name: "IX_cookbook_member__cookbook_id");
        builder.HasIndex(
            member => member.CreatedBy, 
            name: "IX_cookbook_member__created_by");

        builder.Property(member => member.Id)
            .HasColumnName("cookbook_member_id")
            .IsRequired();
        builder.Property(member => member.CookbookId)
            .HasColumnName("cookbook_id")
            .IsRequired();
        builder.Property(member => member.IsCreator)
            .HasColumnName("is_creator")
            .IsRequired();
        builder.Property(member => member.CanAddRecipe)
            .HasColumnName("can_add_recipe")
            .IsRequired();
        builder.Property(member => member.CanUpdateRecipe)
            .HasColumnName("can_update_recipe")
            .IsRequired();
        builder.Property(member => member.CanDeleteRecipe)
            .HasColumnName("can_delete_recipe")
            .IsRequired();
        builder.Property(member => member.CanSendInvite)
            .HasColumnName("can_send_invite")
            .IsRequired();
        builder.Property(member => member.CanRemoveMember)
            .HasColumnName("can_remove_member")
            .IsRequired();
        builder.Property(member => member.CanEditCookbookDetails)
            .HasColumnName("can_edit_cookbook_details")
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany(user => user.CookbookMemberships)
            .HasForeignKey(member => member.CreatedBy)
            .HasConstraintName("FK_cookbook_member__created_by")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.HasOne(member => member.Cookbook)
            .WithMany(cookbook => cookbook.CookbookMembers)
            .HasForeignKey(member => member.CookbookId)
            .HasConstraintName("FK_cookbook_member__cookbook_id")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
