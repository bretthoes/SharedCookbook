﻿namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class RecipeIngredientDto
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required int Ordinal { get; init; }

    public required bool Optional { get; init; }
}
