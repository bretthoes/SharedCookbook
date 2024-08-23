using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("recipe_ingredient");

        builder.HasKey(ri => ri.Id)
            .HasName("PK_recipe_ingredient_id");

        builder.HasIndex(
            ri => ri.RecipeId,
            "IX_recipe_ingredient__recipe_id");

        builder.Property(ri => ri.Id)
            .HasColumnName("recipe_ingredient_id")
            .IsRequired();
        builder.Property(ri => ri.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();
        builder.Property(ri => ri.IngredientName)
            .HasMaxLength(255)
            .HasColumnName("ingredient_name")
            .IsRequired();
        builder.Property(ri => ri.Ordinal)
            .HasColumnName("ordinal")
            .IsRequired();
        builder.Property(ri => ri.Optional)
            .HasColumnName("optional")
            .IsRequired();

        builder.HasOne<Recipe>()
            .WithMany(r => r.Ingredients)
            .HasForeignKey(ri => ri.RecipeId)
            .HasConstraintName("FK_recipe_ingredient__recipe_id");
    }
}
