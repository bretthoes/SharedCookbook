using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class RecipeDirectionConfiguration : IEntityTypeConfiguration<RecipeDirection>
{
    public void Configure(EntityTypeBuilder<RecipeDirection> builder)
    {
        builder.ToTable("recipe_direction");

        builder.HasKey(rd => rd.Id)
            .HasName("PK_recipe_direction_id");

        builder.HasIndex(
            rd => rd.RecipeId,
            name: "IX_recipe_direction__recipe_id");

        builder.Property(rd => rd.Id)
            .HasColumnName("recipe_direction_id")
            .IsRequired();
        builder.Property(rd => rd.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();
        builder.Property(rd => rd.Text)
            .HasMaxLength(255)
            .HasColumnName("text")
            .IsRequired();
        builder.Property(rd => rd.Ordinal)
            .HasColumnName("ordinal")
            .IsRequired();
        builder.Property(rd => rd.Image)
            .HasMaxLength(255)
            .HasColumnName("image");

        builder.HasOne<Recipe>()
            .WithMany(p => p.Directions)
            .HasForeignKey(rd => rd.RecipeId)
            .HasConstraintName("FK_recipe_direction__recipe_id");
    }
}
