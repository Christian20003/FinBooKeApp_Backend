using Moq;

namespace FinBookeAPI.Tests.Authentication;

public partial class AuthenticationServiceUnitTests
{
    /* [Fact]
    public async Task Should_VerifyRefreshToken_BeforeAccessTokenIsGenerated()
    {
        await _service.IssueJwtToken(_token.Value);

        _tokenService.Verify(obj => obj.VerifyRefreshToken(_token.Value), Times.Once);
    }

    [Fact]
    public async Task Should_FailVerificationOfRefreshToken_WhenTokenStoredInDatabase()
    {
        _tokenService.Setup(obj => obj.TokenExists(_token.Value)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.IssueJwtToken(_token.Value));
    }

    [Fact]
    public async Task Should_GenerateNewJwtToken_WhenValidationWasSuccessful()
    {
        _tokenService.Setup(obj => obj.GenerateAccessToken(It.IsAny<string>())).Returns(_token);
        var result = await _service.IssueJwtToken(_token.Value);

        Assert.Equal(_token.Value, result.Value);
        Assert.Equal(_token.Expires, result.Expires);
    } */
}
