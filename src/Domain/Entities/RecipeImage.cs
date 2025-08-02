﻿namespace SharedCookbook.Domain.Entities;

public sealed class RecipeImage : BaseAuditableEntity
{
    public int RecipeId { get; init; }

    public required string Name { get; init; }

    public required int Ordinal { get; init; }

    public struct Constraints
    {
        public const int NameMaxLength = 2048;
    }
}
