using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class RecipeDirectionConfiguration : IEntityTypeConfiguration<RecipeDirection>
{
    public void Configure(EntityTypeBuilder<RecipeDirection> builder)
    {
        builder.ToTable("recipe_direction");

        builder.HasKey(recipeDirection => recipeDirection.Id)
            .HasName("PK_recipe_direction_id");

        builder.HasIndex(
            recipeDirection => recipeDirection.RecipeId,
            name: "IX_recipe_direction__recipe_id");

        builder.Property(recipeDirection => recipeDirection.Id)
            .HasColumnName("recipe_direction_id")
            .IsRequired();
        builder.Property(recipeDirection => recipeDirection.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();
        builder.Property(recipeDirection => recipeDirection.Text)
            .HasMaxLength(RecipeDirection.Constraints.TextMaxLength)
            .HasColumnName("text")
            .IsRequired();
        builder.Property(recipeDirection => recipeDirection.Ordinal)
            .HasColumnName("ordinal")
            .IsRequired();
        builder.Property(recipeDirection => recipeDirection.Image)
            .HasMaxLength(RecipeDirection.Constraints.ImageMaxLength)
            .HasColumnName("image");

        builder.HasOne<Recipe>()
            .WithMany(recipe => recipe.Directions)
            .HasForeignKey(recipeDirection => recipeDirection.RecipeId)
            .HasConstraintName("FK_recipe_direction__recipe_id");
    }
}
