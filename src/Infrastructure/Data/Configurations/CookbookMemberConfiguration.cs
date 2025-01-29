using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class CookbookMemberConfiguration : IEntityTypeConfiguration<CookbookMember>
{
    public void Configure(EntityTypeBuilder<CookbookMember> builder)
    {
        builder.ToTable("cookbook_member");

        builder.HasKey(cm => cm.Id)
            .HasName("PK_cookbook_member_id");

        builder.HasIndex(
            cm => cm.CookbookId, 
            "IX_cookbook_member__cookbook_id");
        builder.HasIndex(
            cm => cm.CreatedBy, 
            "IX_cookbook_member__created_by");

        builder.Property(cm => cm.Id)
            .HasColumnName("cookbook_member_id")
            .IsRequired();
        builder.Property(cm => cm.CookbookId)
            .HasColumnName("cookbook_id")
            .IsRequired();
        builder.Property(cm => cm.IsCreator)
            .HasColumnName("is_creator")
            .IsRequired();
        builder.Property(cm => cm.CanAddRecipe)
            .HasColumnName("can_add_recipe")
            .IsRequired();
        builder.Property(cm => cm.CanUpdateRecipe)
            .HasColumnName("can_update_recipe")
            .IsRequired();
        builder.Property(cm => cm.CanDeleteRecipe)
            .HasColumnName("can_delete_recipe")
            .IsRequired();
        builder.Property(cm => cm.CanSendInvite)
            .HasColumnName("can_send_invite")
            .IsRequired();
        builder.Property(cm => cm.CanRemoveMember)
            .HasColumnName("can_remove_member")
            .IsRequired();
        builder.Property(cm => cm.CanEditCookbookDetails)
            .HasColumnName("can_edit_cookbook_details")
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany(p => p.CookbookMemberships)
            .HasForeignKey(d => d.CreatedBy)
            .HasConstraintName("FK_cookbook_member__created_by")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.HasOne(cm => cm.Cookbook)
            .WithMany(c => c.CookbookMembers)
            .HasForeignKey(cm => cm.CookbookId)
            .HasConstraintName("FK_cookbook_member__cookbook_id")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
