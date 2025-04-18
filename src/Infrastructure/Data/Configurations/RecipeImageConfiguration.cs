using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class RecipeImageConfiguration : IEntityTypeConfiguration<RecipeImage>
{
    public void Configure(EntityTypeBuilder<RecipeImage> builder)
    {
        builder.ToTable("recipe_image");

        builder.HasKey(recipeImage => recipeImage.Id)
            .HasName("PK_recipe_image_id");

        builder.HasIndex(recipeImage => recipeImage.RecipeId,
            name: "IX_recipe_image__recipe_id");

        builder.Property(recipeImage => recipeImage.Id)
            .HasColumnName("recipe_image_id")
            .IsRequired();

        builder.Property(recipeImage => recipeImage.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();

        builder.Property(recipeImage => recipeImage.Name)
            .HasMaxLength(RecipeImage.Constraints.NameMaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(recipeImage => recipeImage.Ordinal)
            .HasColumnName("ordinal")
            .IsRequired();

        builder.HasOne<Recipe>()
            .WithMany(recipe => recipe.Images)
            .HasForeignKey(recipeImage => recipeImage.RecipeId)
            .HasConstraintName("FK_recipe_image__recipe_id");
    }
}
