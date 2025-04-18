using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("recipe_ingredient");

        builder.HasKey(recipeIngredient => recipeIngredient.Id)
            .HasName("PK_recipe_ingredient_id");

        builder.HasIndex(
            recipeIngredient => recipeIngredient.RecipeId,
            name: "IX_recipe_ingredient__recipe_id");

        builder.Property(recipeIngredient => recipeIngredient.Id)
            .HasColumnName("recipe_ingredient_id")
            .IsRequired();
        builder.Property(recipeIngredient => recipeIngredient.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();
        builder.Property(recipeIngredient => recipeIngredient.Name)
            .HasMaxLength(RecipeIngredient.Constraints.NameMaxLength)
            .HasColumnName("name")
            .IsRequired();
        builder.Property(recipeIngredient => recipeIngredient.Ordinal)
            .HasColumnName("ordinal")
            .IsRequired();
        builder.Property(recipeIngredient => recipeIngredient.Optional)
            .HasColumnName("optional")
            .IsRequired();

        builder.HasOne<Recipe>()
            .WithMany(recipe => recipe.Ingredients)
            .HasForeignKey(recipeIngredient => recipeIngredient.RecipeId)
            .HasConstraintName("FK_recipe_ingredient__recipe_id");
    }
}
