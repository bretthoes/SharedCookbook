using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Common;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class InvitationTokenConfiguration : IEntityTypeConfiguration<InvitationToken>
{
    public void Configure(EntityTypeBuilder<InvitationToken> builder)
    {
        builder.ToTable("invitation_token");
        
        builder.HasKey(token => token.Id)
            .HasName("invitation_token_id");
        
        builder.HasIndex(token => new { token.PublicId })
            .HasDatabaseName("IX_invitation_token__public_id");

        builder.Property(token => token.Id)
            .HasColumnName("invitation_token_id")
            .IsRequired();
        builder.Property(token => token.PublicId)
            .HasColumnName("public_id")
            .IsRequired();
        builder.OwnsOne(token => token.Digest, owned =>
        {
            owned.Property(digest => digest.Hash).HasColumnName("token_hash").HasColumnType("bytea").IsRequired();
            owned.Property(digest => digest.Salt).HasColumnName("token_salt").HasColumnType("bytea").IsRequired();
        });
        builder.Property(invitation => invitation.CookbookId)
            .HasColumnName("cookbook_id")
            .IsRequired();
        builder.Property(invitation => invitation.RedeemerPersonId)
            .HasColumnName("redeemer_person_id");
        builder.Property(invitation => invitation.Status)
            .HasColumnName("invitation_status")
            .HasConversion<string>()
            .HasMaxLength(BaseInvitation.Constraints.StatusMaxLength)
            .IsRequired();
        builder.Property(invitation => invitation.ResponseDate)
            .HasColumnName("response_date");

        builder.HasOne(invitation => invitation.Cookbook)
            .WithMany(cookbook => cookbook.Tokens)
            .HasForeignKey(invitation => invitation.CookbookId)
            .HasConstraintName("FK_invitation_token__cookbook_id");
        builder.HasOne<Identity.ApplicationUser>()
            .WithMany()
            .HasForeignKey(invitation => invitation.CreatedBy)
            .HasConstraintName("FK_invitation_token__created_by");
    }
}
