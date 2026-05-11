namespace FinBookeAPI.Tests.Token;

public partial class TokenServiceUnitTests
{
    /* [Fact]
    public void Should_FailToGenerateAccessToken_WhenSecretIsNull()
    {
        _options.AccessTokenSecret = null;

        Assert.Throws<ApplicationException>(() => _service.GenerateAccessToken(_userId));
    }

    [Fact]
    public void Should_FailToGenerateAccessToken_WhenIssuerIsNull()
    {
        _options.Issuer = null;

        Assert.Throws<ApplicationException>(() => _service.GenerateAccessToken(_userId));
    }

    [Fact]
    public void Should_FailToGenerateAccessToken_WhenAudienceIsNull()
    {
        _options.Audience = null;

        Assert.Throws<ApplicationException>(() => _service.GenerateAccessToken(_userId));
    } */

    [Fact]
    public void Should_FailToGenerateAccessToken_WhenSecretIsTooShort()
    {
        _options.AccessTokenSecret = "1234";

        Assert.Throws<ApplicationException>(() => _service.GenerateAccessToken(_userId));
    }

    /* [Fact]
    public void Should_FailToGenerateAccessToken_WhenExpirationTimeIsSmallerThanZero()
    {
        _options.AccessTokenExpireM = 0;

        Assert.Throws<ApplicationException>(() => _service.GenerateAccessToken(_userId));
    } */

    [Fact]
    public void Should_GenerateNotEmptyAccessToken()
    {
        var token = _service.GenerateAccessToken(_userId);

        Assert.NotEqual("", token.Value);
    }

    [Fact]
    public void Should_GenerateAccessTokenWhichExpires()
    {
        var token = _service.GenerateAccessToken(_userId);
        var time = DateTime.UtcNow;

        Assert.True(token.Expires > time.Ticks);
    }
}
