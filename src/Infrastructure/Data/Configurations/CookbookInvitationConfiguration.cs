using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class CookbookInvitationConfiguration : IEntityTypeConfiguration<CookbookInvitation>
{
    public void Configure(EntityTypeBuilder<CookbookInvitation> builder)
    {
        builder.ToTable("cookbook_invitation");

        builder.HasKey(invitation => invitation.Id)
            .HasName("PK_cookbook_invitation_id");

        builder.HasIndex(
            invitation => invitation.CookbookId, 
            name: "IX_cookbook_invitation__cookbook_id");
        builder.HasIndex(
            invitation => invitation.RecipientPersonId,
            name: "IX_cookbook_invitation__recipient_person_id");
        builder.HasIndex(
            invitation => invitation.CreatedBy,
            name: "IX_cookbook_invitation__created_by");

        builder.Property(invitation => invitation.Id)
            .HasColumnName("cookbook_invitation_id")
            .IsRequired();
        builder.Property(invitation => invitation.CookbookId)
            .HasColumnName("cookbook_id")
            .IsRequired();
        builder.Property(invitation => invitation.RecipientPersonId)
            .HasColumnName("recipient_person_id");
        builder.Property(invitation => invitation.InvitationStatus)
            .HasColumnName("invitation_status")
            .HasConversion<string>()
            .HasMaxLength(CookbookInvitation.Constraints.InvitationStatusMaxLength)
            .IsRequired();
        builder.Property(invitation => invitation.ResponseDate)
            .HasColumnName("response_date");
        builder.Property(invitation => invitation.Hash)
            .HasColumnName("hash")
            .HasMaxLength(CookbookInvitation.Constraints.HashLength);
        builder.Property(invitation => invitation.Salt)
            .HasColumnName("salt")
            .HasMaxLength(CookbookInvitation.Constraints.SaltLength);

        builder.HasOne(invitation => invitation.Cookbook)
            .WithMany(cookbook => cookbook.Invitations)
            .HasForeignKey(invitation => invitation.CookbookId)
            .HasConstraintName("FK_cookbook_invitation__cookbook_id");
        builder.HasOne<Identity.ApplicationUser>()
            .WithMany(user => user.ReceivedInvitations)
            .HasForeignKey(invitation => invitation.RecipientPersonId)
            .HasConstraintName("FK_cookbook_invitation__recipient_person_id");
        builder.HasOne<Identity.ApplicationUser>()
            .WithMany(user => user.SentInvitations)
            .HasForeignKey(invitation => invitation.CreatedBy)
            .HasConstraintName("FK_cookbook_invitation__created_by");
    }
}
