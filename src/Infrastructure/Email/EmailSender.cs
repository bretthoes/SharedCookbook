using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace SharedCookbook.Infrastructure.Email;

public class EmailSender(IOptions<EmailApiOptions> options, ILogger<EmailSender> logger) : IEmailSender
{
    private readonly EmailApiOptions _options = options.Value;
    

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var options = new RestClientOptions(_options.BaseUrl)
        {
            Authenticator = new HttpBasicAuthenticator(username: "api", password: _options.ApiKey)
        };

        using var client = new RestClient(options);

        var request = new RestRequest(resource: $"v3/{_options.Domain}/messages", Method.Post);

        // add email parameters to request
        request.AddParameter(name: "from", _options.From);
        request.AddParameter(name: "to", email);
        request.AddParameter(name: "subject", subject);
        request.AddParameter(name: "html", htmlMessage);

        var response = await client.ExecuteAsync(request);
        
        if (!response.IsSuccessful)
            logger.LogError(
                "SharedCookbook Email failed to send: {StatusCode} {Content} {Exception}",
                 response.StatusCode,
                 response.Content,
                 response.ErrorException?.Message);
    }
}
