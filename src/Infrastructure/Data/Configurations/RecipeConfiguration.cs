﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(r => r.Id)
            .HasName("PK_recipe_id");

        builder.ToTable("recipe");

        builder.HasIndex(
            r => r.CookbookId,
            "IX_recipe__cookbook_id");

        builder.Property(r => r.Id)
            .HasColumnName("recipe_id")
            .IsRequired();
        builder.Property(r => r.CookbookId)
            .HasColumnName("cookbook_id")
            .IsRequired();
        builder.Property(r => r.Title)
            .HasMaxLength(255)
            .HasColumnName("title")
            .IsRequired();
        builder.Property(r => r.Summary)
            .HasColumnName("summary");
        builder.Property(r => r.Thumbnail)
            .HasMaxLength(255)
            .HasColumnName("thumbnail");
        builder.Property(r => r.VideoPath)
            .HasMaxLength(255)
            .HasColumnName("video_path");
        builder.Property(r => r.PreparationTimeInMinutes)
            .HasColumnName("preparation_time_in_minutes");
        builder.Property(r => r.CookingTimeInMinutes)
            .HasColumnName("cooking_time_in_minutes");
        builder.Property(r => r.BakingTimeInMinutes)
            .HasColumnName("baking_time_in_minutes");


        builder.HasOne(r => r.Cookbook)
            .WithMany(c => c.Recipes)
            .HasForeignKey(r => r.CookbookId)
            .HasConstraintName("FK_recipe__cookbook_id")
            .IsRequired();
        builder.HasOne<ApplicationUser>()
            .WithMany(a => a.Recipes)
            .HasForeignKey(r => r.CreatedBy)
            .HasConstraintName("FK_recipe__created_by");
        builder.HasMany(r => r.CookbookNotifications)
            .WithOne(cn => cn.Recipe)
            .HasForeignKey(cn => cn.RecipeId)
            .HasConstraintName("FK_cookbook_notification__recipe_id");
        builder.HasMany(r => r.IngredientCategories)
            .WithOne()
            .HasForeignKey(cn => cn.RecipeId)
            .HasConstraintName("FK_ingredient_category__recipe_id");
        builder.HasMany(r => r.Directions)
            .WithOne()
            .HasForeignKey(rd => rd.RecipeId)
            .HasConstraintName("FK_recipe_direction__recipe_id");
        builder.HasMany(r => r.Ingredients)
            .WithOne()
            .HasForeignKey(ri => ri.RecipeId)
            .HasConstraintName("FK_recipe_ingredient__recipe_id");
        builder.HasOne(r => r.Nutrition)
            .WithOne()
            .HasForeignKey<RecipeNutrition>(n => n.RecipeId)
            .HasConstraintName("FK_recipe_nutrition__recipe_id");
    }
}
