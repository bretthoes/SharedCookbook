using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SharedCookbook.Infrastructure.Email;

public class EmailSender(
    IHttpClientFactory clientFactory,
    IOptions<EmailApiOptions> options,
    ILogger<EmailSender> logger) : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = clientFactory.CreateClient();
        client.BaseAddress = new Uri(options.Value.BaseUrl);

        byte[] authBytes = Encoding.ASCII.GetBytes($"api:{options.Value.ApiKey}");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: "Basic", Convert.ToBase64String(authBytes));

        var form = new Dictionary<string, string>
        {
            ["from"] = options.Value.From,
            ["to"] = email,
            ["subject"] = subject,
            ["html"] = htmlMessage
        };

        using var content = new FormUrlEncodedContent(form);
        using var response = await client.PostAsync($"v3/{options.Value.Domain}/messages", content);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();

            logger.LogError(
                "SharedCookbook Email failed to send: {StatusCode} {ReasonPhrase} {Content}",
                (int)response.StatusCode,
                response.ReasonPhrase,
                body);
        }
    }
}
