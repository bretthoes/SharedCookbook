using SharedCookbook.Application.Users.Commands.ExternalLogin;

namespace SharedCookbook.Application.UnitTests.Users.Commands;

public class WhenExternalLoginProviderIsUnknown
{
    private bool _isValid;

    [OneTimeSetUp]
    public void OneTimeSetup() =>
        _isValid = new ExternalLoginCommandValidator()
            .Validate(new ExternalLoginCommand(0, "token"))
            .IsValid;

    [Test]
    public void IsInvalid() => Assert.That(_isValid, Is.False);
}

public class WhenExternalLoginProviderIsGoogle
{
    private bool _isValid;

    [OneTimeSetUp]
    public void OneTimeSetup() =>
        _isValid = new ExternalLoginCommandValidator()
            .Validate(new ExternalLoginCommand(ExternalLoginProvider.Google, "token"))
            .IsValid;

    [Test]
    public void IsValid() => Assert.That(_isValid, Is.True);
}

public class WhenExternalLoginTokenIsBlank
{
    private bool _isValid;

    [OneTimeSetUp]
    public void OneTimeSetup() =>
        _isValid = new ExternalLoginCommandValidator()
            .Validate(new ExternalLoginCommand(ExternalLoginProvider.Apple, " "))
            .IsValid;

    [Test]
    public void IsInvalid() => Assert.That(_isValid, Is.False);
}
