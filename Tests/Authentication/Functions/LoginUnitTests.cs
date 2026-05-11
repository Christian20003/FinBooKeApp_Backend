using System.Security.Authentication;
using FinBookeAPI.Models.Database.Authentication;
using FinBookeAPI.Models.Exceptions;
using FinBookeAPI.Tests.Records;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace FinBookeAPI.Tests.Authentication;

public partial class AuthenticationServiceUnitTests
{
    [Fact]
    public async Task Should_FailAuthentication_WhenEmailIsInvalid()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.Login("invalidEmail", _userAccount.PasswordHash!)
        );
    }

    [Fact]
    public async Task Should_FailAuthentication_WhenEmailIsNotStored()
    {
        await Assert.ThrowsAsync<InvalidCredentialException>(
            () => _service.Login("test@gmx.de", _userAccount.PasswordHash!)
        );
    }

    [Fact]
    public async Task Should_FailAuthentication_WhenPasswordIsInvalid()
    {
        _signInManager
            .Setup(obj =>
                obj.CheckPasswordSignInAsync(
                    It.IsAny<UserAccount>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()
                )
            )
            .ReturnsAsync(SignInResult.Failed);

        await Assert.ThrowsAsync<InvalidCredentialException>(
            () => _service.Login(_userAccount.Email!, _userAccount.PasswordHash!)
        );
    }

    [Fact]
    public async Task Should_FailAuthentication_WhenUserAccountIsLocked()
    {
        _signInManager
            .Setup(obj =>
                obj.CheckPasswordSignInAsync(
                    It.IsAny<UserAccount>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()
                )
            )
            .ReturnsAsync(SignInResult.LockedOut);

        await Assert.ThrowsAsync<ResourceLockedException>(
            () => _service.Login(_userAccount.Email!, _userAccount.PasswordHash!)
        );
    }

    [Fact]
    public async Task Should_FailAuthentication_WhenUserAccountIsRevoked()
    {
        _userAccount.IsRevoked = true;

        await Assert.ThrowsAsync<ResourceLockedException>(
            () => _service.Login(_userAccount.Email!, _userAccount.PasswordHash!)
        );
    }

    [Fact]
    public async Task Should_ReturnJwtTokens_WhenAuthenticationIsSuccessful()
    {
        var result = await _service.Login(_userAccount.Email!, _userAccount.PasswordHash!);
        var expected = JwtTokenRecord.GetObject();

        Assert.Equal(expected.Value, result.AccessToken.Value);
        Assert.True(result.AccessToken.Expires > 0);
        Assert.Equal(expected.Value, result.RefreshToken.Value);
        Assert.True(result.RefreshToken.Expires > 0);
    }

    [Fact]
    public async Task Should_ReturnUserData_WhenAuthenticationIsSuccessful()
    {
        var result = await _service.Login(_userAccount.Email!, _userAccount.PasswordHash!);

        Assert.Equal(_userAccount.Id, result.Id.ToString());
        Assert.Equal(_userAccount.UserName, result.Name);
        Assert.Equal(_userAccount.Email, result.Email);
    }
}
