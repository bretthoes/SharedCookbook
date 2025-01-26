using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace SharedCookbook.Infrastructure.Email;

public class EmailSender(IOptions<EmailApiOptions> options) : IEmailSender
{
    private readonly EmailApiOptions _options = options.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var authenticator = new HttpBasicAuthenticator("api", _options.ApiKey);

        var options = new RestClientOptions(_options.BaseUrl)
        {
            Authenticator = authenticator
        };

        var client = new RestClient(options);

        var request = new RestRequest
        {
            Resource = "{domain}/messages",
            Method = Method.Post
        };

        // add email parameters to request
        request.AddParameter("domain", _options.Domain, ParameterType.UrlSegment);
        request.AddParameter("from", _options.From);
        request.AddParameter("to", email);
        request.AddParameter("subject", subject);
        request.AddParameter("html", htmlMessage);

        await client.ExecuteAsync(request);
    }
}
