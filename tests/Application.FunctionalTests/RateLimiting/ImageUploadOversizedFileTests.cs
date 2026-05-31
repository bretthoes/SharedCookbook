using System.Net;

namespace SharedCookbook.Application.FunctionalTests.RateLimiting;

[NonParallelizable]
public class WhenImageFileExceedsMaxSize
{
    private HttpClient _client = null!;
    private HttpResponseMessage _response = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _client = RateLimitFixture.Factory.CreateClient();
        _response = await ImageUploadRateLimitTestClient.UploadOversizedImageAsync(
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
    public void Upload_ReturnsPayloadTooLarge() =>
        Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.RequestEntityTooLarge));
}
