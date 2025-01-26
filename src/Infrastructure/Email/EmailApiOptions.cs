namespace SharedCookbook.Infrastructure.Email;

public class EmailApiOptions
{
    public required string ApiKey { get; init; }
    public required string BaseUrl { get; init; }
    public required string Domain { get; init; }
    public required string From { get; init; }
}
