namespace SharedCookbook.Application.FunctionalTests.RateLimiting;

[NonParallelizable]
[SetUpFixture]
public class RateLimitFixture
{
    private static ITestDatabase _database = null!;

    internal static RateLimitWebApplicationFactory Factory { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task RunBeforeRateLimitTests()
    {
        _database = await TestDatabaseFactory.CreateAsync();
        Factory = new RateLimitWebApplicationFactory(_database.GetConnection());
    }

    [OneTimeTearDown]
    public async Task RunAfterRateLimitTests()
    {
        await _database.DisposeAsync();
        await Factory.DisposeAsync();
    }
}
