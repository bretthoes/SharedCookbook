using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class CookbookInvitationConfiguration : IEntityTypeConfiguration<CookbookInvitation>
{
    public void Configure(EntityTypeBuilder<CookbookInvitation> builder)
    {
        builder.ToTable("cookbook_invitation");

        builder.HasKey(ci => ci.Id)
            .HasName("PK_cookbook_invitation_id");

        builder.HasIndex(
            ci => ci.CookbookId, 
            "IX_cookbook_invitation__cookbook_id");
        builder.HasIndex(
            ci => ci.RecipientPersonId,
            "IX_cookbook_invitation__recipient_person_id");
        builder.HasIndex(
            ci => ci.CreatedBy,
            "IX_cookbook_invitation__created_by");

        builder.Property(ci => ci.Id)
            .HasColumnName("cookbook_invitation_id")
            .IsRequired();
        builder.Property(ci => ci.CookbookId)
            .HasColumnName("cookbook_id")
            .IsRequired();
        builder.Property(ci => ci.RecipientPersonId)
            .HasColumnName("recipient_person_id");
        builder.Property(ci => ci.InvitationStatus)
            .HasColumnName("invitation_status")
            .HasConversion<string>()
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(ci => ci.ResponseDate)
            .HasColumnName("response_date");

        builder.HasOne(ci => ci.Cookbook)
            .WithMany(c => c.CookbookInvitations)
            .HasForeignKey(ci => ci.CookbookId)
            .HasConstraintName("FK_cookbook_invitation__cookbook_id");
        builder.HasOne<Identity.ApplicationUser>()
            .WithMany(p => p.ReceivedInvitations)
            .HasForeignKey(ci => ci.RecipientPersonId)
            .HasConstraintName("FK_cookbook_invitation__recipient_person_id");
        builder.HasOne<Identity.ApplicationUser>()
            .WithMany(p => p.SentInvitations)
            .HasForeignKey(ci => ci.CreatedBy)
            .HasConstraintName("FK_cookbook_invitation__created_by");
    }
}
