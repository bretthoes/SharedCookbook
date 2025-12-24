using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class IngredientCategoryConfiguration : IEntityTypeConfiguration<IngredientCategory>
{
    public void Configure(EntityTypeBuilder<IngredientCategory> builder)
    {
        builder.ToTable("ingredient_category");

        builder.HasKey(category => category.Id)
            .HasName("PK_ingredient_category_id");

        builder.HasIndex(
            category => category.RecipeId,
            name: "IX_ingredient__category_recipe_id");

        builder.Property(category => category.Id)
            .HasColumnName("ingredient_category_id")
            .IsRequired();
        builder.Property(category => category.RecipeId)
            .HasColumnName("recipe_id");
        builder.Property(category => category.Title)
            .HasMaxLength(IngredientCategory.Constraints.TitleMaxLength)
            .HasColumnName("title")
            .IsRequired();

        builder.HasOne<Recipe>()
            .WithMany()
            .HasForeignKey(category => category.RecipeId)
            .HasConstraintName("FK_ingredient_category__recipe_id")
            .IsRequired();
    }
}
