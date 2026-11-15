using System.Net;

namespace SharedCookbook.Application.FunctionalTests.RateLimiting;

internal static class AccountEmailRateLimitTestClient
{
    private const string ResendConfirmationEmailPath = "/api/Users/resendConfirmationEmail";

    internal static Task<HttpResponseMessage> ResendConfirmationEmailAsync(HttpClient client, string clientIp)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, ResendConfirmationEmailPath)
        {
            Content = JsonContent.Create(new { email = "nobody@test.local" }),
        };
        request.Headers.Add("X-Forwarded-For", clientIp);

        return client.SendAsync(request);
    }
}

[NonParallelizable]
public class WhenClientIsWithinAccountEmailLimit
{
    private const string ClientIp = "203.0.113.10";
    private HttpClient _client = null!;
    private HttpResponseMessage _response = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _client = RateLimitFixture.Factory.CreateClient();
        _response = await AccountEmailRateLimitTestClient.ResendConfirmationEmailAsync(_client, ClientIp);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _response.Dispose();
        _client.Dispose();
    }

    [Test]
    public void FirstRequest_ReturnsSuccessStatusCode() =>
        Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
}

[NonParallelizable]
public class WhenClientExceedsAccountEmailLimit
{
    private const string ClientIp = "203.0.113.20";
    private HttpClient _client = null!;
    private HttpResponseMessage _lastResponse = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _client = RateLimitFixture.Factory.CreateClient();

        for (var i = 0; i < RateLimitWebApplicationFactory.TestDailyLimit; i++)
        {
            (await AccountEmailRateLimitTestClient.ResendConfirmationEmailAsync(_client, ClientIp)).Dispose();
        }

        _lastResponse = await AccountEmailRateLimitTestClient.ResendConfirmationEmailAsync(_client, ClientIp);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _lastResponse.Dispose();
        _client.Dispose();
    }

    [Test]
    public void RequestOverLimit_ReturnsTooManyRequests() =>
        Assert.That(_lastResponse.StatusCode, Is.EqualTo(HttpStatusCode.TooManyRequests));
}

[NonParallelizable]
public class WhenAnotherClientHasIndependentAccountEmailLimit
{
    private const string ExhaustedClientIp = "203.0.113.30";
    private const string OtherClientIp = "203.0.113.31";
    private HttpClient _client = null!;
    private HttpResponseMessage _otherClientResponse = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _client = RateLimitFixture.Factory.CreateClient();

        for (var i = 0; i < RateLimitWebApplicationFactory.TestDailyLimit; i++)
        {
            (await AccountEmailRateLimitTestClient.ResendConfirmationEmailAsync(_client, ExhaustedClientIp)).Dispose();
        }

        _otherClientResponse = await AccountEmailRateLimitTestClient.ResendConfirmationEmailAsync(_client, OtherClientIp);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _otherClientResponse.Dispose();
        _client.Dispose();
    }

    [Test]
    public void OtherClientFirstRequest_ReturnsSuccessStatusCode() =>
        Assert.That(_otherClientResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
}
