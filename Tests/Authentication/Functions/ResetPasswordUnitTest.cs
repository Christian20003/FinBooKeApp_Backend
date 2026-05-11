using System.Security.Authentication;
using FinBookeAPI.Models.Database.Authentication;
using FinBookeAPI.Models.Exceptions;
using FinBookeAPI.Services.Authentication;
using Moq;

namespace FinBookeAPI.Tests.Authentication;

public partial class AuthenticationServiceUnitTests
{
    [Fact]
    public async Task Should_FailResetPassword_WhenEmailIsInvalid()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.ResetPassword("invalidEmail", _userAccount.AccessCode!)
        );
    }

    [Fact]
    public async Task Should_FailResetPassword_WhenEmailHasNotAnAccount()
    {
        _userManager
            .Setup(obj => obj.GetUsersAsync())
            .Returns(new List<UserAccount>().ToAsyncEnumerable());

        await Assert.ThrowsAsync<InvalidCredentialException>(
            () => _service.ResetPassword(_userAccount.Email!, _userAccount.AccessCode!)
        );
    }

    [Fact]
    public async Task Should_FailResetPassword_WhenAccessCodeIsNull()
    {
        _userAccount.AccessCode = null;

        await Assert.ThrowsAsync<AuthorizationException>(
            () => _service.ResetPassword(_userAccount.Email!, "H6Z7IK")
        );
    }

    [Fact]
    public async Task Should_FailResetPassword_WhenAccessCodeCreatedAtIsNull()
    {
        _userAccount.AccessCodeCreatedAt = null;

        await Assert.ThrowsAsync<AuthorizationException>(
            () => _service.ResetPassword(_userAccount.Email!, _userAccount.AccessCode!)
        );
    }

    [Fact]
    public async Task Should_FailResetPassword_WhenAccessCodeIsMoreThan10MinutesOld()
    {
        _userAccount.AccessCodeCreatedAt = DateTime.UtcNow.AddMinutes(-12);

        await Assert.ThrowsAsync<AuthorizationException>(
            () => _service.ResetPassword(_userAccount.Email!, _userAccount.AccessCode!)
        );
    }

    [Fact]
    public async Task Should_FailResetPassword_WhenAccessCodeIsInvalid()
    {
        await Assert.ThrowsAsync<AuthorizationException>(
            () => _service.ResetPassword(_userAccount.Email!, "H6Z7IK")
        );
    }

    [Fact]
    public async Task Should_GenerateAndStoreNewPassword()
    {
        await _service.ResetPassword(_userAccount.Email!, _userAccount.AccessCode!);

        _securityUtilityService.Verify(obj => obj.GeneratePassword(20), Times.Once);
        _userManager.Verify(
            obj => obj.SetPasswordAsync(It.IsAny<UserAccount>(), It.IsAny<string>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Should_UpdateUserAccount_WhenResetPasswordWasSuccessful()
    {
        await _service.ResetPassword(_userAccount.Email!, _userAccount.AccessCode!);

        _userManager.Verify(obj => obj.UpdateUserAsync(It.IsAny<UserAccount>()), Times.Once);
    }

    /* [Fact]
    public async Task Should_FailResetPassword_WhenTemplateFileNotFound()
    {
        typeof(AuthenticationService)
            .GetField("TemplateFilePwd", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!
            .SetValue(null, "nonexistent.html");

        await Assert.ThrowsAsync<ApplicationException>(() => _service.ResetPassword(_userAccount.Email!, _userAccount.AccessCode!));
    } */

    [Fact]
    public async Task Should_SendEmail_WhenResetPasswordWasSuccessful()
    {
        await _service.ResetPassword(_userAccount.Email!, _userAccount.AccessCode!);

        _emailService.Verify(
            obj => obj.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true),
            Times.Once
        );
    }
}
