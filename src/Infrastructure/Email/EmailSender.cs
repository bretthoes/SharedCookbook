using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace SharedCookbook.Infrastructure.Email;

// TODO inject RestClient
public class EmailSender(IOptions<EmailApiOptions> options, ILogger<EmailSender> logger) : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var clientOptions = new RestClientOptions(options.Value.BaseUrl)
        {
            Authenticator = new HttpBasicAuthenticator(username: "api", password: options.Value.ApiKey)
        };

        using var client = new RestClient(clientOptions);

        var request = new RestRequest(resource: $"v3/{options.Value.Domain}/messages", Method.Post);

        // add email parameters to request
        request.AddParameter(name: "from", options.Value.From);
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
