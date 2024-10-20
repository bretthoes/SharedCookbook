﻿using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class RecipeDirectionDto
{
    public required int Id { get; set; }

    public required string Text { get; set; }

    public required int Ordinal { get; set; }

    public string? Image { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<RecipeDirection, RecipeDirectionDto>();
        }
    }
}
