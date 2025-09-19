using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class InvitationTokenConfiguration : IEntityTypeConfiguration<InvitationToken>
{
    public void Configure(EntityTypeBuilder<InvitationToken> builder)
    {
        builder.ToTable("invitation_token");
        
        builder.HasKey(token => token.Id)
            .HasName("PK_cookbook_invitation_id");
        
        builder.HasIndex(token => new { token.CookbookInvitationId, token.Status })
            .HasDatabaseName("IX_invitation_token__invitation_status");

        builder.Property(token => token.Id)
            .HasColumnName("invitation_token_id")
            .IsRequired();
        builder.Property(token => token.CookbookInvitationId)
            .HasColumnName("cookbook_invitation_id")
            .IsRequired();
        builder.Property(token => token.Hash)
            .HasColumnName("token_hash")
            .IsRequired();
        builder.Property(token => token.Salt)
            .HasColumnName("token_salt")
            .IsRequired();
        builder.Property(token => token.Status)
            .HasColumnName("token_status")
            .HasConversion<string>()
            .HasMaxLength(InvitationToken.Constraints.InvitationTokenStatusMaxLength)
            .IsRequired();

        builder.HasOne(token => token.Invitation)
            .WithMany(invitation => invitation.Tokens)
            .HasForeignKey(token => token.CookbookInvitationId)
            .HasConstraintName("FK_cookbook_invitation__invitation_token");
    }
}
