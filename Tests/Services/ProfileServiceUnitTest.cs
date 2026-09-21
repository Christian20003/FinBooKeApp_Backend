using FinBooKeAPI.Collections.AccountCollection;
using FinBooKeAPI.Logic.Email;
using FinBooKeAPI.Logic.FileSystem;
using FinBooKeAPI.Logic.Security;
using FinBooKeAPI.Models.Database.Account;
using FinBookeAPI.Models.Result;
using FinBooKeAPI.Models.Settings;
using FinBookeAPI.Services.Profile;
using FinBooKeAPI.Services.Profile;
using FinBooKeAPI.Test.Mocks.Collections;
using FinBooKeAPI.Tests.Mocks.Dependencies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Moq;

namespace FinBooKeAPI.Tests.Services;

public class ProfileServiceUnitTest
{
    private readonly Mock<IAccountCollection> _accountCollection;
    private readonly Mock<IUploadSystem> _upload;
    private readonly Mock<IHashProvider> _hashProvider;
    private readonly Mock<IDataProtection> _protection;
    private readonly Mock<IEmailProvider> _emailProvider;
    private readonly Mock<IEmailTemplateBuilder> _emailBuilder;
    private readonly Mock<IStringLocalizer<ProfileService>> _localizer;
    private readonly Mock<IOptions<AccountSettings>> _accountSettings;
    private readonly Mock<IOptions<SmtpSettings>> _smtpSettings;

    private readonly ProfileService _service;

    private readonly MockAccountCollection.InMemoryCollection _collection;
    private readonly MockUploadSystem.InMemoryFileSystem _fileSystem;
    private readonly MockEmailProvider.InMemorySmtpServer _smtp;
    private UserAccount _user;

    public ProfileServiceUnitTest()
    {
        _collection = MockAccountCollection.GetCollection();
        _fileSystem = new MockUploadSystem.InMemoryFileSystem();
        _smtp = new MockEmailProvider.InMemorySmtpServer();
        _user = _collection.Accounts.First();

        _accountCollection = MockAccountCollection.GetMock(_collection);
        _upload = MockUploadSystem.GetMock(_fileSystem);
        _hashProvider = MockHashProvider.GetMock();
        _protection = MockDataProtection.GetMock();
        _emailProvider = MockEmailProvider.GetMock(_smtp);
        _emailBuilder = MockEmailTemplateBuilder.GetMock();
        _localizer = MockStringLocalizer.GetMock<ProfileService>();
        _accountSettings = MockAccountSettings.GetMock();
        _smtpSettings = MockSmtpSettings.GetMock();
        var logger = new Mock<ILogger<ProfileService>>();

        _service = new ProfileService(
            _accountCollection.Object,
            _upload.Object,
            _hashProvider.Object,
            _protection.Object,
            _emailProvider.Object,
            _emailBuilder.Object,
            _localizer.Object,
            _accountSettings.Object,
            _smtpSettings.Object,
            logger.Object
        );
    }

    [Fact]
    public async Task GetChangeEmailTokenAsync_WhenInvalidGuid_ReturnsError()
    {
        var result = await _service.GetChangeEmailTokenAsync(Guid.NewGuid(), "newEmail");

        Assert.False(result.HasValue());
        Assert.Equal(ServiceResultCode.USER_NOT_FOUND, result.ErrorCode);
    }

    [Fact]
    public async Task GetChangeEmailTokenAsync_WhenNewEmailIdentical_ReturnsError()
    {
        var id = Guid.Parse(_user.Id);
        var result = await _service.GetChangeEmailTokenAsync(id, _user.Email!);

        Assert.False(result.HasValue());
        Assert.Equal(ServiceResultCode.EMAIL_IDENTICAL, result.ErrorCode);
    }

    [Fact]
    public async Task GetChangeEmailTokenAsync_WhenUpdateFailed_ReturnsError()
    {
        _accountCollection
            .Setup(obj => obj.UpdateAccountAsync(It.IsAny<UserAccount>()))
            .ReturnsAsync(IdentityResult.Failed([]));

        var id = Guid.Parse(_user.Id);
        var result = await _service.GetChangeEmailTokenAsync(id, "newEmail");

        Assert.False(result.HasValue());
        Assert.Equal(ServiceResultCode.USER_UPDATE_FAILED, result.ErrorCode);
    }

    [Fact]
    public async Task GetChangeEmailTokenAsync_WhenTokenCreated_SendEmail()
    {
        var id = Guid.Parse(_user.Id);
        var result = await _service.GetChangeEmailTokenAsync(id, "newEmail");

        Assert.True(result.HasValue());
        Assert.NotEmpty(_smtp.Mails);
    }

    [Fact]
    public async Task GetChangeEmailTokenAsync_WhenTokenCreated_SendValidEmailPayload()
    {
        var id = Guid.Parse(_user.Id);
        var result = await _service.GetChangeEmailTokenAsync(id, "newEmail");

        Assert.NotEmpty(_smtp.Mails);
        var payload = _smtp.Mails.First();
        Assert.NotEmpty(payload.Body);
        Assert.NotEmpty(payload.From);
        Assert.NotEmpty(payload.Host);
        Assert.NotEmpty(payload.Password);
        Assert.NotEqual(0, payload.Port);
        Assert.NotEmpty(payload.Subject);
        Assert.NotEmpty(payload.To);
    }

    [Fact]
    public async Task ChangeEmailAsync_WhenUserNotFound_ReturnError()
    {
        var id = Guid.NewGuid();
        var result = await _service.ChangeEmailAsync(id, "", "");

        Assert.False(result.HasValue());
        Assert.Equal(ServiceResultCode.USER_NOT_FOUND, result.ErrorCode);
    }

    [Fact]
    public async Task ChangeEmailAsync_WhenEmailNotEqual_ReturnError()
    {
        var id = Guid.Parse(_user.Id);
        var result = await _service.ChangeEmailAsync(id, "", "other@gmail.com");

        Assert.False(result.HasValue());
        Assert.Equal(ServiceResultCode.EMAIL_INVALID, result.ErrorCode);
    }

    [Fact]
    public async Task ChangeEmailAsync_WhenTokenNotValid_ReturnError()
    {
        _user.ChangeEmailHash = "other@gmail.com";
        var id = Guid.Parse(_user.Id);
        var result = await _service.ChangeEmailAsync(id, "invalidToken", _user.ChangeEmailHash);

        Assert.False(result.HasValue());
        Assert.Equal(ServiceResultCode.TOKEN_INVALID, result.ErrorCode);
    }

    [Fact]
    public async Task ChangeEmailAsync_WhenTokenValid_ReturnSuccess()
    {
        _user.ChangeEmailHash = "other@gmail.com";
        _collection.ChangeEmailTokens.Add(_user.ChangeEmailHash, "myToken");
        var id = Guid.Parse(_user.Id);
        var result = await _service.ChangeEmailAsync(id, "myToken", _user.ChangeEmailHash);

        Assert.True(result.HasValue());
    }

    [Fact]
    public async Task ChangeEmailAsync_WhenTokenValid_UpdateEmailAddress()
    {
        var oldEmail = _user.Email;
        _user.ChangeEmailHash = "other@gmail.com";
        _collection.ChangeEmailTokens.Add(_user.ChangeEmailHash, "myToken");
        var id = Guid.Parse(_user.Id);
        var result = await _service.ChangeEmailAsync(id, "myToken", _user.ChangeEmailHash);

        Assert.NotEqual(oldEmail, _user.Email);
        Assert.Equal(_user.ChangeEmailHash, _user.Email);
    }

    [Fact]
    public async Task DeleteProfileImage_WhenInvalidUserId_ReturnError()
    {
        var id = Guid.NewGuid();
        var result = await _service.DeleteProfileImageAsync(id, "");

        Assert.False(result.HasValue());
        Assert.Equal(ServiceResultCode.USER_NOT_FOUND, result.ErrorCode);
    }

    [Fact]
    public async Task DeleteProfileImage_WhenAccountUpdateFailed_ReturnError()
    {
        _accountCollection
            .Setup(obj => obj.UpdateAccountAsync(It.IsAny<UserAccount>()))
            .ReturnsAsync(IdentityResult.Failed([]));
        var id = Guid.Parse(_user.Id);

        var result = await _service.DeleteProfileImageAsync(id, "");

        Assert.False(result.HasValue());
        Assert.Equal(ServiceResultCode.USER_UPDATE_FAILED, result.ErrorCode);
    }

    [Fact]
    public async Task DeleteProfileImage_WhenDeleteSucceeded_ReturnSuccess()
    {
        var id = Guid.Parse(_user.Id);
        var result = await _service.DeleteProfileImageAsync(id, "");

        Assert.True(result.HasValue());
    }

    [Fact]
    public async Task DeleteProfileImage_WhenDeleteSucceeded_RemovePathValueFromAccount()
    {
        var id = Guid.Parse(_user.Id);
        var result = await _service.DeleteProfileImageAsync(id, "");

        Assert.Equal(string.Empty, _user.ImagePath);
    }

    [Fact]
    public async Task DeleteProfileImage_WhenDeleteSucceeded_RemoveImageFromFileSystem()
    {
        _fileSystem.Files.Add("myFile.jpg", "myFileContent");
        var id = Guid.Parse(_user.Id);
        var result = await _service.DeleteProfileImageAsync(id, "myFile.jpg");

        Assert.Empty(_fileSystem.Files);
    }

    [Fact]
    public async Task GetVerifyEmailTokenAsync_WhenInvalidUserId_ReturnError()
    {
        var id = Guid.NewGuid();
        var result = await _service.GetVerifyEmailTokenAsync(id);

        Assert.False(result.HasValue());
        Assert.Equal(ServiceResultCode.USER_NOT_FOUND, result.ErrorCode);
    }

    [Fact]
    public async Task GetVerifyEmailTokenAsync_WhenTokenCreated_SendEmail()
    {
        var id = Guid.Parse(_user.Id);
        var result = await _service.GetVerifyEmailTokenAsync(id);

        Assert.True(result.HasValue());
        Assert.NotEmpty(_smtp.Mails);
    }

    [Fact]
    public async Task GetVerifyEmailTokenAsync_WhenTokenCreated_SendValidEmailPayload()
    {
        var id = Guid.Parse(_user.Id);
        var result = await _service.GetVerifyEmailTokenAsync(id);

        Assert.NotEmpty(_smtp.Mails);
        var payload = _smtp.Mails.First();
        Assert.NotEmpty(payload.Body);
        Assert.NotEmpty(payload.From);
        Assert.NotEmpty(payload.Host);
        Assert.NotEmpty(payload.Password);
        Assert.NotEqual(0, payload.Port);
        Assert.NotEmpty(payload.Subject);
        Assert.NotEmpty(payload.To);
    }
}
