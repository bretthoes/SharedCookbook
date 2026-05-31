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
            notification => notification.RecipientUserId,
            name: "IX_cookbook_notification__recipient_user_id");
        builder.HasIndex(
            notification => new { notification.RecipientUserId, notification.Created },
            name: "IX_cookbook_notification__recipient_created");
        builder.HasIndex(
            notification => notification.CookbookId,
            name: "IX_cookbook_notification__cookbook_id");
        builder.HasIndex(
            notification => notification.RecipeId,
            name: "IX_cookbook_notification__recipe_id");

        builder.Property(notification => notification.Id)
            .HasColumnName("cookbook_notification_id")
            .IsRequired();
        builder.Property(notification => notification.RecipientUserId)
            .HasColumnName("recipient_user_id")
            .IsRequired();
        builder.Property(notification => notification.CookbookId)
            .HasColumnName("cookbook_id")
            .IsRequired();
        builder.Property(notification => notification.RecipeId)
            .HasColumnName("recipe_id");
        builder.Property(notification => notification.ActionType)
            .HasColumnName("action_type")
            .HasConversion<string>()
            .HasMaxLength(CookbookNotification.Constraints.ActionTypeMaxLength)
            .IsRequired();
        builder.Property(notification => notification.ActorUserId)
            .HasColumnName("actor_user_id");
        builder.Property(notification => notification.SubjectUserId)
            .HasColumnName("subject_user_id");
        builder.Property(notification => notification.Created)
            .HasColumnName("created")
            .IsRequired();

        builder.HasOne(notification => notification.Cookbook)
            .WithMany(cookbook => cookbook.Notifications)
            .HasForeignKey(notification => notification.CookbookId)
            .HasConstraintName("FK_cookbook_notification__cookbook_id")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.HasOne(notification => notification.Recipe)
            .WithMany()
            .HasForeignKey(notification => notification.RecipeId)
            .HasConstraintName("FK_cookbook_notification__recipe_id")
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(notification => notification.RecipientUserId)
            .HasConstraintName("FK_cookbook_notification__recipient_user_id")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(notification => notification.ActorUserId)
            .HasConstraintName("FK_cookbook_notification__actor_user_id")
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(notification => notification.SubjectUserId)
            .HasConstraintName("FK_cookbook_notification__subject_user_id")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
