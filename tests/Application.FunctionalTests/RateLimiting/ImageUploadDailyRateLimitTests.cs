using System.Net;

namespace SharedCookbook.Application.FunctionalTests.RateLimiting;

[NonParallelizable]
public class WhenUserIsWithinDailyImageUploadLimit
{
    private HttpClient _client = null!;
    private HttpResponseMessage _response = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _client = RateLimitFixture.Factory.CreateClient();
        _response = await ImageUploadRateLimitTestClient.UploadImageAsync(
            _client,
            userId: Guid.NewGuid().ToString());
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
public class WhenUserExceedsDailyImageUploadLimit
{
    private HttpClient _client = null!;
    private HttpResponseMessage _thirdResponse = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        var userId = Guid.NewGuid().ToString();
        _client = RateLimitFixture.Factory.CreateClient();

        (await ImageUploadRateLimitTestClient.UploadImageAsync(_client, userId)).Dispose();
        (await ImageUploadRateLimitTestClient.UploadImageAsync(_client, userId)).Dispose();
        _thirdResponse = await ImageUploadRateLimitTestClient.UploadImageAsync(_client, userId);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _thirdResponse.Dispose();
        _client.Dispose();
    }

    [Test]
    public void ThirdRequest_ReturnsTooManyRequests() =>
        Assert.That(_thirdResponse.StatusCode, Is.EqualTo(HttpStatusCode.TooManyRequests));
}

[NonParallelizable]
public class WhenAnotherUserHasIndependentDailyImageUploadLimit
{
    private HttpClient _client = null!;
    private HttpResponseMessage _otherUserResponse = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        var exhaustedUserId = Guid.NewGuid().ToString();
        var otherUserId = Guid.NewGuid().ToString();
        _client = RateLimitFixture.Factory.CreateClient();

        for (var i = 0; i < RateLimitWebApplicationFactory.TestDailyLimit; i++)
        {
            (await ImageUploadRateLimitTestClient.UploadImageAsync(_client, exhaustedUserId)).Dispose();
        }

        _otherUserResponse = await ImageUploadRateLimitTestClient.UploadImageAsync(_client, otherUserId);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _otherUserResponse.Dispose();
        _client.Dispose();
    }

    [Test]
    public void OtherUserFirstRequest_ReturnsSuccessStatusCode() =>
        Assert.That(_otherUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
}
