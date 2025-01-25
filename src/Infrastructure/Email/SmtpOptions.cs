namespace SharedCookbook.Infrastructure.Email;

public class SmtpOptions
{
    public required string Host { get; init; }
    public required int Port { get; init; }
}
