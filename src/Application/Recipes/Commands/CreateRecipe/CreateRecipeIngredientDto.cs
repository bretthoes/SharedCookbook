﻿using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class CreateRecipeIngredientDto
{
    public required string Name { get; set; }

    public required int Ordinal { get; set; }

    public required bool Optional { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<RecipeIngredient, CreateRecipeIngredientDto>();
        }
    }
}