using FinBookeAPI.Models.Database.Authentication;
using FinBookeAPI.Models.Exceptions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace FinBookeAPI.Tests.Authentication;

public partial class AuthenticationServiceUnitTests
{
    [Fact]
    public async Task Should_FailRegistration_WhenUserNameIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.Register(_userAccount.Email!, "", _userAccount.PasswordHash!)
        );
    }

    [Fact]
    public async Task Should_FailRegistration_WhenEmailIsInvalid()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () =>
                _service.Register(
                    "invalidemail",
                    _userAccount.UserName!,
                    _userAccount.PasswordHash!
                )
        );
    }

    [Fact]
    public async Task Should_ProtectEmailAndUserName()
    {
        await _service.Register(
            _userAccount.Email!,
            _userAccount.UserName!,
            _userAccount.PasswordHash!
        );

        _dataProtection.Verify(obj => obj.Protect(_userAccount.UserName!), Times.Once);
        _dataProtection.Verify(obj => obj.ProtectEmail(_userAccount.Email!), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Should_FailRegistration_WhenUserAccountConditionsAreViolated()
    {
        _userManager
            .Setup(obj => obj.CreateUserAsync(It.IsAny<UserAccount>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        await Assert.ThrowsAsync<IdentityResultException>(
            () =>
                _service.Register(
                    _userAccount.Email!,
                    _userAccount.UserName!,
                    _userAccount.PasswordHash!
                )
        );
    }

    [Fact]
    public async Task Should_GenerateRefreshAndAccessTokens_WhenRegistrationWasSuccessful()
    {
        await _service.Register(
            _userAccount.Email!,
            _userAccount.UserName!,
            _userAccount.PasswordHash!
        );

        _tokenService.Verify(obj => obj.GenerateAccessToken(It.IsAny<string>()), Times.Once);
        _tokenService.Verify(obj => obj.GenerateRefreshToken(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnUserData_WhenRegistrationWasSuccessful()
    {
        var result = await _service.Register(
            _userAccount.Email!,
            _userAccount.UserName!,
            _userAccount.PasswordHash!
        );

        Assert.Equal(_userAccount.UserName, result.Name);
        Assert.Equal(_userAccount.Email, result.Email);
        Assert.Equal(_userAccount.ImagePath, result.ImagePath);
        Assert.NotEqual(Guid.Empty, result.Id);
    }
}
