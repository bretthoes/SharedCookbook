using SharedCookbook.Domain.Exceptions;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.ValueObjects;

public class TokenDigestTests
{
    [TestCase(0)]
    [TestCase(16)]
    [TestCase(31)]
    [TestCase(33)]
    public void ThrowsWhenHashLengthIsInvalid(int hashLength)
    {
        Assert.Throws<InvalidHashLengthException>(
            () => _ = new TokenDigest(hash: new byte[hashLength], salt: It.IsAny<byte[]>()));
    }
    
    [TestCase(0)]
    [TestCase(15)]
    [TestCase(17)]
    [TestCase(32)]
    public void ThrowsWhenSaltLengthIsInvalid(int saltLength)
    {
        Assert.Throws<InvalidSaltLengthException>(() =>
            _ = new TokenDigest(hash: new byte[TokenDigest.HashLength], salt: new byte[saltLength]));
    }

    [Test]
    public void ValidHashIsExpectedLength()
    {
        var sut = new TokenDigest(new byte[TokenDigest.HashLength], new byte[TokenDigest.SaltLength]);
        
        Assert.That(sut.Hash, Has.Length.EqualTo(TokenDigest.HashLength));
    }

    [Test]
    public void ValidSaltIsExpectedLength()
    {
        var sut = new TokenDigest(new byte[TokenDigest.HashLength], new byte[TokenDigest.SaltLength]);
        
        Assert.That(sut.Salt, Has.Length.EqualTo(TokenDigest.SaltLength));
    }
}
