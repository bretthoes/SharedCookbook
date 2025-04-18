using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(recipe => recipe.Id)
            .HasName("PK_recipe_id");

        builder.ToTable("recipe");

        builder.HasIndex(
            recipe => recipe.CookbookId,
            name: "IX_recipe__cookbook_id");

        builder.Property(recipe => recipe.Id)
            .HasColumnName("recipe_id")
            .IsRequired();
        builder.Property(recipe => recipe.CookbookId)
            .HasColumnName("cookbook_id")
            .IsRequired();
        builder.Property(recipe => recipe.Title)
            .HasMaxLength(Recipe.Constraints.TitleMaxLength)
            .HasColumnName("title")
            .IsRequired();
        builder.Property(recipe => recipe.Summary)
            .HasMaxLength(Recipe.Constraints.SummaryMaxLength)
            .HasColumnName("summary");
        builder.Property(recipe => recipe.Thumbnail)
            .HasMaxLength(Recipe.Constraints.ThumbnailMaxLength)
            .HasColumnName("thumbnail");
        builder.Property(recipe => recipe.VideoPath)
            .HasMaxLength(Recipe.Constraints.VideoPathMaxLength)
            .HasColumnName("video_path");
        builder.Property(recipe => recipe.PreparationTimeInMinutes)
            .HasColumnName("preparation_time_in_minutes");
        builder.Property(recipe => recipe.CookingTimeInMinutes)
            .HasColumnName("cooking_time_in_minutes");
        builder.Property(recipe => recipe.BakingTimeInMinutes)
            .HasColumnName("baking_time_in_minutes");


        builder.HasOne(recipe => recipe.Cookbook)
            .WithMany(cookbook => cookbook.Recipes)
            .HasForeignKey(recipe => recipe.CookbookId)
            .HasConstraintName("FK_recipe__cookbook_id")
            .IsRequired();
        builder.HasOne<ApplicationUser>()
            .WithMany(user => user.Recipes)
            .HasForeignKey(recipe => recipe.CreatedBy)
            .HasConstraintName("FK_recipe__created_by");
        builder.HasMany(recipe => recipe.CookbookNotifications)
            .WithOne(cn => cn.Recipe)
            .HasForeignKey(cn => cn.RecipeId)
            .HasConstraintName("FK_cookbook_notification__recipe_id");
        builder.HasMany(recipe => recipe.IngredientCategories)
            .WithOne()
            .HasForeignKey(category => category.RecipeId)
            .HasConstraintName("FK_ingredient_category__recipe_id");
        builder.HasMany(recipe => recipe.Directions)
            .WithOne()
            .HasForeignKey(rd => rd.RecipeId)
            .HasConstraintName("FK_recipe_direction__recipe_id");
        builder.HasMany(recipe => recipe.Ingredients)
            .WithOne()
            .HasForeignKey(ingredient => ingredient.RecipeId)
            .HasConstraintName("FK_recipe_ingredient__recipe_id");
        builder.HasOne(recipe => recipe.Nutrition)
            .WithOne()
            .HasForeignKey<RecipeNutrition>(nutrition => nutrition.RecipeId)
            .HasConstraintName("FK_recipe_nutrition__recipe_id");
    }
}
