namespace FinBookeAPI.Tests.Token;

public partial class TokenServiceUnitTests
{
    /* [Fact]
    public void Should_FailToGenerateRefreshToken_WhenSecretIsNull()
    {
        _options.RefreshTokenSecret = null;

        Assert.Throws<ApplicationException>(() => _service.GenerateRefreshToken(_userId));
    }

    [Fact]
    public void Should_FailToGenerateRefreshToken_WhenIssuerIsNull()
    {
        _options.Issuer = null;

        Assert.Throws<ApplicationException>(() => _service.GenerateRefreshToken(_userId));
    }

    [Fact]
    public void Should_FailToGenerateRefreshToken_WhenAudienceIsNull()
    {
        _options.Audience = null;

        Assert.Throws<ApplicationException>(() => _service.GenerateRefreshToken(_userId));
    } */

    [Fact]
    public void Should_FailToGenerateRefreshToken_WhenSecretIsTooShort()
    {
        _options.RefreshTokenSecret = "1234";

        Assert.Throws<ApplicationException>(() => _service.GenerateRefreshToken(_userId));
    }

    /* [Fact]
    public void Should_FailToGenerateRefreshToken_WhenExpirationTimeIsSmallerThanZero()
    {
        _options.RefreshTokenExpireD = 0;

        Assert.Throws<ApplicationException>(() => _service.GenerateRefreshToken(_userId));
    } */

    [Fact]
    public void Should_GenerateNotEmptyRefreshToken()
    {
        var token = _service.GenerateRefreshToken(_userId);

        Assert.NotEqual("", token.Value);
    }

    [Fact]
    public void Should_GenerateRefreshTokenWhichExpires()
    {
        var token = _service.GenerateRefreshToken(_userId);
        var time = DateTime.UtcNow;

        Assert.True(token.Expires > time.Ticks);
    }
}
