namespace SharedCookbook.Application.FunctionalTests;

using static Testing;

[TestFixture]
[NonParallelizable]
public abstract class BaseTestFixture
{
    [SetUp]
    public async Task TestSetUp()
    {
        await ResetState();
        await RunAsDefaultUserAsync();
    }
}
