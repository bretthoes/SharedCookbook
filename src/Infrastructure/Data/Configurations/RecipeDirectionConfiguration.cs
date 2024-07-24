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
            "IX_recipe_direction__recipe_id");

        builder.Property(rd => rd.Id)
            .HasColumnName("recipe_direction_id")
            .IsRequired();
        builder.Property(rd => rd.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();
        builder.Property(rd => rd.DirectionText)
            .HasMaxLength(255)
            .HasColumnName("instruction")
            .IsRequired();
        builder.Property(rd => rd.Ordinal)
            .HasColumnName("ordinal")
            .IsRequired();
        builder.Property(rd => rd.ImagePath)
            .HasMaxLength(255)
            .HasColumnName("image_path");

        builder.HasOne<Recipe>()
            .WithMany(p => p.RecipeDirections)
            .HasForeignKey(rd => rd.RecipeId)
            .HasConstraintName("FK_recipe_direction__recipe_id");
    }
}
