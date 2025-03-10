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

        builder.HasKey(notification => notification.Id)
            .HasName("PK_cookbook_notification_id");

        builder.HasIndex(
            notification => notification.CreatedBy,
            name: "IX_cookbook_notification__created_by");
        builder.HasIndex(
            notification => notification.CookbookId,
            name: "IX_cookbook_notification__cookbook_id");
        builder.HasIndex(
            notification => notification.RecipeId,
            name: "IX_cookbook_notification__recipe_id");

        builder.Property(notification => notification.Id)
            .HasColumnName("cookbook_notification_id")
            .IsRequired();
        builder.Property(notification => notification.CookbookId)
            .HasColumnName("cookbook_id");
        builder.Property(notification => notification.RecipeId)
            .HasColumnName("recipe_id");
        builder.Property(notification => notification.ActionType)
            .HasColumnName("action_type")
            .HasConversion<string>()
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(notification => notification.Created)
            .HasColumnName("created")
            .IsRequired();

        builder.HasOne(notification => notification.Cookbook)
            .WithMany(cookbook => cookbook.CookbookNotifications)
            .HasForeignKey(notification => notification.CookbookId)
            .HasConstraintName("FK_cookbook_notification__cookbook_id");
        builder.HasOne(notification => notification.Recipe)
            .WithMany(recipe => recipe.CookbookNotifications)
            .HasForeignKey(notification => notification.RecipeId)
            .HasConstraintName("FK_cookbook_notification__recipe_id");
        builder.HasOne<ApplicationUser>()
            .WithMany(user => user.CookbookNotifications)
            .HasForeignKey(notification => notification.CreatedBy)
            .HasConstraintName("FK_cookbook_notification__created_by");
    }
}
