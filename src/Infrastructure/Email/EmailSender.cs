using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Org.BouncyCastle.Crypto.Digests;
using RestSharp;
using RestSharp.Authenticators;
using Parameter = RestSharp.Parameter;

namespace SharedCookbook.Infrastructure.Email;

public class EmailSender(IOptions<EmailApiOptions> options) : IEmailSender
{
    private readonly EmailApiOptions _options = options.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var options = new RestClientOptions(_options.BaseUrl) {
            Authenticator = new HttpBasicAuthenticator("api", _options.ApiKey)
        };
        var client = new RestClient(options);
        
        var request = new RestRequest();
        request.AddParameter ("domain", _options.Domain, ParameterType.UrlSegment);
        request.Resource = "{domain}/messages";
        request.AddParameter ("from", _options.From);
        request.AddParameter ("to", email);
        request.AddParameter ("subject", subject);
        request.AddParameter ("html", htmlMessage);
        request.Method = Method.Post;
        var result = await client.ExecuteAsync(request);
    }
}
