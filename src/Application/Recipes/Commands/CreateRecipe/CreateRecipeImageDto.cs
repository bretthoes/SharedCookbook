﻿using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public class CreateRecipeImageDto
{
    public required string Name { get; set; }

    public required int Ordinal { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateRecipeImageDto, RecipeImage>();
        }
    }
}
