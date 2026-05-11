using System.Security.Authentication;
using FinBookeAPI.Models.Database.Authentication;
using FinBookeAPI.Services.Authentication;
using Moq;

namespace FinBookeAPI.Tests.Authentication;

public partial class AuthenticationServiceUnitTests
{
    [Fact]
    public async Task Should_FailSendingAccessCode_WhenEmailIsInvalid()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SendAccessCode("invalidEmail"));
    }

    [Fact]
    public async Task Should_FailSendingAccessCode_WhenEmailHasNotAnAccount()
    {
        _userManager
            .Setup(obj => obj.GetUsersAsync())
            .Returns(new List<UserAccount>().ToAsyncEnumerable());

        await Assert.ThrowsAsync<InvalidCredentialException>(
            () => _service.SendAccessCode(_userAccount.Email!)
        );
    }

    [Fact]
    public async Task Should_GenerateAccessCode()
    {
        await _service.SendAccessCode(_userAccount.Email!);

        _securityUtilityService.Verify(obj => obj.GenerateAccessCode(6), Times.Once);
    }

    /* [Fact]
    public async Task Should_FailSendingAccessCode_WhenTemplateFileNotFound()
    {
        typeof(AuthenticationService)
            .GetField("TemplateFileCode", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!
            .SetValue(null, "nonexistent.html");

        await Assert.ThrowsAsync<ApplicationException>(() => _service.SendAccessCode(_userAccount.Email!));
    } */

    [Fact]
    public async Task Should_UpdateUserAccount_WhenAccessCodeIsGenerated()
    {
        await _service.SendAccessCode(_userAccount.Email!);

        _userManager.Verify(obj => obj.UpdateUserAsync(It.IsAny<UserAccount>()), Times.Once);
    }

    [Fact]
    public async Task Should_SendEmail_WhenAccessCodeIsGenerated()
    {
        await _service.SendAccessCode(_userAccount.Email!);

        _emailService.Verify(
            obj => obj.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true),
            Times.Once
        );
    }
}
