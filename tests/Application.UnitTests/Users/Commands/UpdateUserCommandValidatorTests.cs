using SharedCookbook.Application.Users.Commands.UpdateUser;

namespace SharedCookbook.Application.UnitTests.Users.Commands;

public class WhenDisplayNameIsOnlyWhitespace
{
    private const string WhitespaceDisplayName = "   ";
    private bool _isValid;

    [OneTimeSetUp]
    public void OneTimeSetup() =>
        _isValid = new UpdateUserCommandValidator().Validate(new UpdateUserCommand(WhitespaceDisplayName)).IsValid;

    [Test]
    public void IsInvalid() => Assert.That(_isValid, Is.False);
}

public class WhenDisplayNameHasLettersAndApostrophe
{
    private const string ValidDisplayName = "Mary O'Brien";
    private bool _isValid;

    [OneTimeSetUp]
    public void OneTimeSetup() =>
        _isValid = new UpdateUserCommandValidator().Validate(new UpdateUserCommand(ValidDisplayName)).IsValid;

    [Test]
    public void IsValid() => Assert.That(_isValid, Is.True);
}
