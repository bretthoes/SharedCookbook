namespace SharedCookbook.Application.Contracts;

public class CookbookBriefDto
{
    public required int Id { get; init; }

    public required string Title { get; init; }

    public string? Image { get; init; }

    public string? Author { get; set; }
    
    public string? AuthorEmail { get; set; }
    
    public int MembersCount { get; init; }
    
    public int RecipeCount { get; init; }
}
