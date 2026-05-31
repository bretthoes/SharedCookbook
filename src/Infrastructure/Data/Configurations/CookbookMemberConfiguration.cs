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

        builder.HasKey(membership => membership.Id).HasName("PK_cookbook_member_id");

        builder.HasIndex(membership => membership.CookbookId, "IX_cookbook_member__cookbook_id");
        builder.HasIndex(membership => membership.CreatedBy, "IX_cookbook_member__created_by");
        builder.HasIndex(membership => new { membership.CookbookId, membership.CreatedBy }, "UX_cookbook_member__cookbook_user").IsUnique();

        builder.Property(membership => membership.Id)
            .HasColumnName("cookbook_member_id")
            .IsRequired();

        builder.Property(membership => membership.CookbookId)
            .HasColumnName("cookbook_id")
            .IsRequired();

        builder.Property(membership => membership.Tier)
            .HasColumnName("tier")
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(membership => membership.CreatedBy)
            .HasConstraintName("FK_cookbook_member__created_by")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(membership => membership.Cookbook)
            .WithMany(c => c.Memberships)
            .HasForeignKey(membership => membership.CookbookId)
            .HasConstraintName("FK_cookbook_member__cookbook_id")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
