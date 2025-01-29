using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class CookbookNotificationConfiguration : IEntityTypeConfiguration<CookbookNotification>
{
    public void Configure(EntityTypeBuilder<CookbookNotification> builder)
    {
        builder.ToTable("cookbook_notification");

        builder.HasKey(cn => cn.Id)
            .HasName("PK_cookbook_notification_id");

        builder.HasIndex(
            cn => cn.CreatedBy,
            "IX_cookbook_notification__created_by");
        builder.HasIndex(
            cn => cn.CookbookId,
            "IX_cookbook_notification__cookbook_id");
        builder.HasIndex(
            cn => cn.RecipeId,
            "IX_cookbook_notification__recipe_id");

        builder.Property(cn => cn.Id)
            .HasColumnName("cookbook_notification_id")
            .IsRequired();
        builder.Property(cn => cn.CookbookId)
            .HasColumnName("cookbook_id");
        builder.Property(cn => cn.RecipeId)
            .HasColumnName("recipe_id");
        builder.Property(cn => cn.ActionType)
            .HasColumnName("action_type")
            .HasConversion<string>()
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(cn => cn.Created)
            .HasColumnName("created")
            .IsRequired();

        builder.HasOne(cn => cn.Cookbook)
            .WithMany(c => c.CookbookNotifications)
            .HasForeignKey(cn => cn.CookbookId)
            .HasConstraintName("FK_cookbook_notification__cookbook_id");
        builder.HasOne(cn => cn.Recipe)
            .WithMany(r => r.CookbookNotifications)
            .HasForeignKey(cn => cn.RecipeId)
            .HasConstraintName("FK_cookbook_notification__recipe_id");
        builder.HasOne<ApplicationUser>()
            .WithMany(p => p.CookbookNotifications)
            .HasForeignKey(cn => cn.CreatedBy)
            .HasConstraintName("FK_cookbook_notification__created_by");
    }
}
