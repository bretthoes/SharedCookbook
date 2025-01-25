namespace SharedCookbook.Infrastructure.Email;

public class SmtpOptions
{
    public required string FromAddress { get; init; }
    public required string FromAddressName { get; init; }
    public required string Host { get; init; }
    public required int Port { get; init; }
}
