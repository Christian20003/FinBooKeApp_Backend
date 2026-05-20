using Microsoft.IdentityModel.Tokens;
using Moq;

namespace FinBookeAPI.Tests.Authentication;

public partial class AuthenticationServiceUnitTests
{
    /* [Fact]
    public async Task Should_SucceedUserLogout_WhenAccessTokenHasExpired()
    {
        _tokenService
            .Setup(obj => obj.StoreAccessToken(It.IsAny<string>()))
            .ThrowsAsync(new SecurityTokenExpiredException());

        try
        {
            await _service.Logout(_token.Value, _token.Value);
        }
        catch (SecurityTokenExpiredException)
        {
            Assert.Fail("An expired exception has been thrown which should not happen");
        }
        Assert.True(true);
    }

    [Fact]
    public async Task Should_SucceedUserLogout_WhenRefreshTokenHasExpired()
    {
        _tokenService
            .Setup(obj => obj.StoreRefreshToken(It.IsAny<string>()))
            .ThrowsAsync(new SecurityTokenExpiredException());

        try
        {
            await _service.Logout(_token.Value, _token.Value);
        }
        catch (SecurityTokenExpiredException)
        {
            Assert.Fail("An expired exception has been thrown which should not happen");
        }
        Assert.True(true);
    }

    [Fact]
    public async Task Should_SucceedUserLogout_WhenTokensAreValid()
    {
        try
        {
            await _service.Logout(_token.Value, _token.Value);
        }
        catch (Exception)
        {
            Assert.Fail("An exception has been thrown which should not happen");
        }
        Assert.True(true);
    } */
}
