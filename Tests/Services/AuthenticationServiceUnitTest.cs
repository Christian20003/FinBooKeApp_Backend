using System.Linq.Expressions;
using FinBooKeAPI.Collections.AccountCollection;
using FinBooKeAPI.Logic.Authentication;
using FinBooKeAPI.Logic.Email;
using FinBooKeAPI.Logic.Security;
using FinBookeAPI.Models.Database.Authentication;
using FinBooKeAPI.Models.DTO.Authentication;
using FinBooKeAPI.Models.Logic.Authentication;
using FinBooKeAPI.Models.Logic.Email;
using FinBookeAPI.Models.Result;
using FinBooKeAPI.Models.Settings;
using FinBookeAPI.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Moq;

namespace FinBooKeAPI.Tests.Services;

public class AuthenticationServiceUnitTest
{
    private readonly Mock<SignInManager<UserAccount>> _signInManager;
    private readonly Mock<IAccountCollection> _accountCollection;
    private readonly Mock<ITokenProvider> _tokenProvider;
    private readonly Mock<IClaimProvider> _claimProvider;
    private readonly Mock<IDataProtection> _protection;
    private readonly Mock<IHashProvider> _hashProvider;
    private readonly Mock<IEmailProvider> _emailProvider;
    private readonly Mock<IEmailTemplateBuilder> _emailTemplateBuilder;
    private readonly Mock<IOptions<AuthenticationSettings>> _authenticationSettings;
    private readonly Mock<IOptions<SmtpSettings>> _smtpSettings;
    private readonly Mock<IStringLocalizer<AuthenticationService>> _localizer;
    private readonly Mock<ILogger<AuthenticationService>> _logger;

    private readonly AuthenticationService _service;

    private readonly List<UserAccount> _userDatabase = [];
    private readonly List<string> _refreshTokenDatabase = [];

    public AuthenticationServiceUnitTest()
    {
        var userManager = new Mock<UserManager<UserAccount>>(
            Mock.Of<IUserStore<UserAccount>>(),
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<UserAccount>>(),
            FakeItEasy.A.Fake<IEnumerable<IUserValidator<UserAccount>>>(),
            FakeItEasy.A.Fake<IEnumerable<IPasswordValidator<UserAccount>>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<UserAccount>>>()
        );
        _signInManager = new Mock<SignInManager<UserAccount>>(
            userManager.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<UserAccount>>(),
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<ILogger<SignInManager<UserAccount>>>(),
            Mock.Of<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>(),
            Mock.Of<IUserConfirmation<UserAccount>>()
        );

        _accountCollection = new Mock<IAccountCollection>();
        _tokenProvider = new Mock<ITokenProvider>();
        _claimProvider = new Mock<IClaimProvider>();
        _protection = new Mock<IDataProtection>();
        _hashProvider = new Mock<IHashProvider>();
        _emailProvider = new Mock<IEmailProvider>();
        _emailTemplateBuilder = new Mock<IEmailTemplateBuilder>();
        _authenticationSettings = new Mock<IOptions<AuthenticationSettings>>();
        _smtpSettings = new Mock<IOptions<SmtpSettings>>();
        _localizer = new Mock<IStringLocalizer<AuthenticationService>>();
        _logger = new Mock<ILogger<AuthenticationService>>();

        _service = new AuthenticationService(
            _signInManager.Object,
            _accountCollection.Object,
            _tokenProvider.Object,
            _claimProvider.Object,
            _protection.Object,
            _hashProvider.Object,
            _emailProvider.Object,
            _emailTemplateBuilder.Object,
            _authenticationSettings.Object,
            _smtpSettings.Object,
            _localizer.Object,
            _logger.Object
        );
    }

    private static UserAccount GetUserAccount()
    {
        return new UserAccount
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "name",
            Email = "email",
            EmailHash = "email",
            ImagePath = "path",
            PasswordHash = "password",
        };
    }

    private static AuthenticationSettings GetAuthenticationSettings()
    {
        return new AuthenticationSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            AccessTokenSecret = "accessSecret",
            RefreshTokenSecret = "refreshSecret",
            ResetPasswordLink = "link",
        };
    }

    private static SmtpSettings GetSmtpSettings()
    {
        return new SmtpSettings
        {
            Host = "host",
            Port = 1,
            Username = "username",
            Password = "password",
            Address = "host-email",
        };
    }

    private static AuthenticationToken GetAccessToken()
    {
        return new AuthenticationToken { Value = "access", Expires = DateTime.UtcNow.Ticks };
    }

    private static AuthenticationToken GetRefreshToken()
    {
        return new AuthenticationToken { Value = "refresh", Expires = DateTime.UtcNow.Ticks };
    }

    private static EmailPayload GetEmptyEmailPayload()
    {
        return new()
        {
            Host = "",
            Port = 0,
            From = "",
            To = [],
            Subject = "",
            Body = "",
            IsHtml = false,
            Username = "",
            Password = "",
        };
    }

    private static string GetResetPasswordToken()
    {
        return "resetPasswordToken";
    }

    private void SetupLogin()
    {
        var account = GetUserAccount();
        _userDatabase.Add(account);
        _hashProvider
            .Setup(obj => obj.Hash(It.IsAny<string>()))
            .Returns<string>(
                (value) =>
                {
                    return value;
                }
            );
        _accountCollection
            .Setup(obj => obj.GetAccountAsync(It.IsAny<Expression<Func<UserAccount, bool>>>()))
            .ReturnsAsync(
                (Expression<Func<UserAccount, bool>> condition) =>
                {
                    return _userDatabase.FirstOrDefault(condition.Compile());
                }
            );
        _accountCollection
            .Setup(obj =>
                obj.SetAccountRefreshTokenAsync(It.IsAny<UserAccount>(), It.IsAny<string>())
            )
            .Callback<UserAccount, string>(
                (account, token) =>
                {
                    _refreshTokenDatabase.Add(token);
                }
            )
            .ReturnsAsync(IdentityResult.Success);
        _signInManager
            .Setup(obj =>
                obj.CheckPasswordSignInAsync(
                    It.IsAny<UserAccount>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()
                )
            )
            .ReturnsAsync(
                (UserAccount account, string password, bool lockout) =>
                {
                    if (account.PasswordHash != password)
                    {
                        return SignInResult.Failed;
                    }
                    return SignInResult.Success;
                }
            );

        var settings = GetAuthenticationSettings();
        var accessToken = GetAccessToken();
        var refreshToken = GetRefreshToken();
        _tokenProvider
            .Setup(obj => obj.CreateToken(It.IsAny<CreateTokenPayload>()))
            .Returns<CreateTokenPayload>(
                (payload) =>
                {
                    if (payload.Secret == settings.AccessTokenSecret)
                    {
                        return accessToken;
                    }
                    return refreshToken;
                }
            );
        _authenticationSettings.Setup(obj => obj.Value).Returns(settings);

        _protection
            .Setup(obj => obj.UnprotectEmail(It.IsAny<string>()))
            .Returns<string>(
                (value) =>
                {
                    return value;
                }
            );
    }

    private void SetupRegister()
    {
        _accountCollection
            .Setup(obj => obj.CreateAccountAsync(It.IsAny<UserAccount>(), It.IsAny<string>()))
            .ReturnsAsync(
                (UserAccount account, string password) =>
                {
                    _userDatabase.Add(account);
                    return IdentityResult.Success;
                }
            );
        _accountCollection
            .Setup(obj =>
                obj.SetAccountRefreshTokenAsync(It.IsAny<UserAccount>(), It.IsAny<string>())
            )
            .ReturnsAsync(
                (UserAccount account, string token) =>
                {
                    _refreshTokenDatabase.Add(token);
                    return IdentityResult.Success;
                }
            );

        var settings = GetAuthenticationSettings();
        var accessToken = GetAccessToken();
        var refreshToken = GetRefreshToken();
        _tokenProvider
            .Setup(obj => obj.CreateToken(It.IsAny<CreateTokenPayload>()))
            .Returns<CreateTokenPayload>(
                (payload) =>
                {
                    if (payload.Secret == settings.AccessTokenSecret)
                    {
                        return accessToken;
                    }
                    return refreshToken;
                }
            );
        _authenticationSettings.Setup(obj => obj.Value).Returns(settings);

        _protection
            .Setup(obj => obj.UnprotectEmail(It.IsAny<string>()))
            .Returns<string>(
                (value) =>
                {
                    return value;
                }
            );
        _protection
            .Setup(obj => obj.ProtectEmail(It.IsAny<string>()))
            .Returns<string>(
                (value) =>
                {
                    return value;
                }
            );
    }

    private void SetupLogout()
    {
        var account = GetUserAccount();
        _userDatabase.Add(account);
        _accountCollection
            .Setup(obj => obj.GetAccountAsync(It.IsAny<Expression<Func<UserAccount, bool>>>()))
            .ReturnsAsync(
                (Expression<Func<UserAccount, bool>> condition) =>
                {
                    return _userDatabase.FirstOrDefault(condition.Compile());
                }
            );
        _accountCollection
            .Setup(obj => obj.DeleteAccountRefreshTokenAsync(It.IsAny<UserAccount>()))
            .Callback<UserAccount>(
                (account) =>
                {
                    _refreshTokenDatabase.Clear();
                }
            )
            .ReturnsAsync(IdentityResult.Success);
    }

    private void SetupSendResetPasswordToken()
    {
        var account = GetUserAccount();
        var smtpSettings = GetSmtpSettings();
        var authSettings = GetAuthenticationSettings();
        _userDatabase.Add(account);
        _hashProvider
            .Setup(obj => obj.Hash(It.IsAny<string>()))
            .Returns(
                (string value) =>
                {
                    return value;
                }
            );
        _accountCollection
            .Setup(obj => obj.GetAccountAsync(It.IsAny<Expression<Func<UserAccount, bool>>>()))
            .ReturnsAsync(
                (Expression<Func<UserAccount, bool>> condition) =>
                {
                    return _userDatabase.FirstOrDefault(condition.Compile());
                }
            );
        _accountCollection
            .Setup(obj => obj.GeneratePasswordResetTokenAsync(It.IsAny<UserAccount>()))
            .ReturnsAsync(GetResetPasswordToken());
        _authenticationSettings.Setup(obj => obj.Value).Returns(authSettings);
        _smtpSettings.Setup(obj => obj.Value).Returns(smtpSettings);
        _emailTemplateBuilder
            .Setup(obj => obj.GetResetPasswordTemplate(It.IsAny<string>()))
            .Returns(
                (string value) =>
                {
                    return $"template-{value}";
                }
            );
        _localizer
            .Setup(obj => obj[It.IsAny<string>()])
            .Returns(new LocalizedString("subject", "subject"));
    }

    private void SetupResetPasswordToken()
    {
        var account = GetUserAccount();
        _userDatabase.Add(account);
        _hashProvider
            .Setup(obj => obj.Hash(It.IsAny<string>()))
            .Returns(
                (string value) =>
                {
                    return value;
                }
            );
        _accountCollection
            .Setup(obj => obj.GetAccountAsync(It.IsAny<Expression<Func<UserAccount, bool>>>()))
            .ReturnsAsync(
                (Expression<Func<UserAccount, bool>> condition) =>
                {
                    return _userDatabase.FirstOrDefault(condition.Compile());
                }
            );
        _accountCollection
            .Setup(obj =>
                obj.ResetPasswordAsync(
                    It.IsAny<UserAccount>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(
                (UserAccount user, string token, string password) =>
                {
                    var account = _userDatabase.First();
                    account.PasswordHash = password;
                    return IdentityResult.Success;
                }
            );
    }

    [Fact]
    public async Task Should_FailLogin_WhenEmailIsInvalid()
    {
        SetupLogin();
        var loginData = new LoginDTO { Email = "invalidEmail", Password = "password" };

        var result = await _service.LoginAsync(loginData);

        Assert.Equal(ErrorType.BAD_REQUEST, result.ErrorType);
    }

    [Fact]
    public async Task Should_FailLogin_WhenAccountIsLocked()
    {
        SetupLogin();
        var account = GetUserAccount();
        var loginData = new LoginDTO { Email = "email", Password = "password" };
        _signInManager
            .Setup(obj =>
                obj.CheckPasswordSignInAsync(
                    It.IsAny<UserAccount>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()
                )
            )
            .ReturnsAsync(SignInResult.LockedOut);

        var result = await _service.LoginAsync(loginData);

        Assert.Equal(ErrorType.FORBIDDEN, result.ErrorType);
    }

    [Fact]
    public async Task Should_FailLogin_WhenPasswordIsInvalid()
    {
        SetupLogin();
        var loginData = new LoginDTO { Email = "email", Password = "invalidPassword" };

        var result = await _service.LoginAsync(loginData);

        Assert.Equal(ErrorType.BAD_REQUEST, result.ErrorType);
    }

    [Fact]
    public async Task Should_FailLogin_WhenRefreshTokenCouldNotBeStored()
    {
        SetupLogin();
        var loginData = new LoginDTO { Email = "email", Password = "password" };
        _accountCollection
            .Setup(obj =>
                obj.SetAccountRefreshTokenAsync(It.IsAny<UserAccount>(), It.IsAny<string>())
            )
            .ReturnsAsync(IdentityResult.Failed([]));

        var result = await _service.LoginAsync(loginData);

        Assert.Equal(ErrorType.INTERNAL_ERROR, result.ErrorType);
    }

    [Fact]
    public async Task Should_SucceedLogin_WhenCredentialsAreValid()
    {
        SetupLogin();
        var loginData = new LoginDTO { Email = "email", Password = "password" };

        var result = await _service.LoginAsync(loginData);

        Assert.Equal(ErrorType.NONE, result.ErrorType);
        Assert.True(result.HasValue);
    }

    [Fact]
    public async Task Should_ReturnCorrectUserAccount_WhenLoginWasSuccessuful()
    {
        SetupLogin();
        var loginData = new LoginDTO { Email = "email", Password = "password" };

        var result = await _service.LoginAsync(loginData);
        var user = result.Value!;

        Assert.Equal(loginData.Email, user.Email);
    }

    [Fact]
    public async Task Should_ReturnDifferentTokens_WhenLoginWasSuccessuful()
    {
        SetupLogin();
        var loginData = new LoginDTO { Email = "email", Password = "password" };

        var result = await _service.LoginAsync(loginData);
        var user = result.Value!;

        Assert.NotEqual(user.Session.AccessToken, user.Session.RefreshToken);
    }

    [Fact]
    public async Task Should_StoreRefreshToken_WhenLoginWasSuccessuful()
    {
        SetupLogin();
        var loginData = new LoginDTO { Email = "email", Password = "password" };

        var result = await _service.LoginAsync(loginData);

        Assert.Contains(result.Value!.Session.RefreshToken, _refreshTokenDatabase);
    }

    [Fact]
    public async Task Should_FailRegister_WhenCredentialConditionsAreNotMet()
    {
        SetupRegister();
        _accountCollection
            .Setup(obj => obj.CreateAccountAsync(It.IsAny<UserAccount>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed([]));

        var registerData = new RegisterDTO
        {
            Username = "username",
            Email = "email",
            Password = "password",
        };
        var result = await _service.RegisterAsync(registerData);

        Assert.Equal(ErrorType.BAD_REQUEST, result.ErrorType);
        Assert.Empty(_userDatabase);
    }

    [Fact]
    public async Task Should_FailRegister_WhenRefreshTokenCouldNotBeStored()
    {
        SetupRegister();
        _accountCollection
            .Setup(obj =>
                obj.SetAccountRefreshTokenAsync(It.IsAny<UserAccount>(), It.IsAny<string>())
            )
            .ReturnsAsync(IdentityResult.Failed([]));

        var registerData = new RegisterDTO
        {
            Username = "username",
            Email = "email",
            Password = "password",
        };
        var result = await _service.RegisterAsync(registerData);

        Assert.Equal(ErrorType.INTERNAL_ERROR, result.ErrorType);
        Assert.Empty(_refreshTokenDatabase);
    }

    [Fact]
    public async Task Should_SucceedRegister_WhenCredentialsAreValid()
    {
        SetupRegister();

        var registerData = new RegisterDTO
        {
            Username = "username",
            Email = "email",
            Password = "password",
        };
        var result = await _service.RegisterAsync(registerData);

        Assert.Equal(ErrorType.NONE, result.ErrorType);
    }

    [Fact]
    public async Task Should_ReturnCorrectUserAccount_WhenRegisterWasSuccessuful()
    {
        SetupRegister();

        var registerData = new RegisterDTO
        {
            Username = "username",
            Email = "email",
            Password = "password",
        };
        var result = await _service.RegisterAsync(registerData);

        Assert.Equal(registerData.Email, result.Value!.Email);
        Assert.Equal(registerData.Username, result.Value!.Name);
    }

    [Fact]
    public async Task Should_StoreNewUserAccount_WhenRegisterWasSuccessuful()
    {
        SetupRegister();

        var registerData = new RegisterDTO
        {
            Username = "username",
            Email = "email",
            Password = "password",
        };
        var result = await _service.RegisterAsync(registerData);

        var account = _userDatabase.First();
        Assert.Equal(registerData.Email, account.Email);
        Assert.Equal(registerData.Username, account.UserName);
    }

    [Fact]
    public async Task Should_ReturnDifferentTokens_WhenRegisterWasSuccessuful()
    {
        SetupRegister();

        var registerData = new RegisterDTO
        {
            Username = "username",
            Email = "email",
            Password = "password",
        };
        var result = await _service.RegisterAsync(registerData);

        Assert.NotEqual(result.Value!.Session.AccessToken, result.Value!.Session.RefreshToken);
    }

    [Fact]
    public async Task Should_StoreRefreshToken_WhenRegisterWasSuccessuful()
    {
        SetupRegister();

        var registerData = new RegisterDTO
        {
            Username = "username",
            Email = "email",
            Password = "password",
        };
        var result = await _service.RegisterAsync(registerData);

        var token = _refreshTokenDatabase.First();
        Assert.Equal(token, result.Value!.Session.RefreshToken);
    }

    [Fact]
    public async Task Should_FailLogout_WhenUserAccountIsNotFound()
    {
        SetupLogout();

        var result = await _service.LogoutAsync(Guid.Empty);

        Assert.Equal(ErrorType.UNAUTHORIZED, result.ErrorType);
    }

    [Fact]
    public async Task Should_FailLogout_WhenDeleteOperationOfRefreshTokenFailed()
    {
        SetupLogout();
        _accountCollection
            .Setup(obj => obj.DeleteAccountRefreshTokenAsync(It.IsAny<UserAccount>()))
            .ReturnsAsync(IdentityResult.Failed([]));

        var user = _userDatabase.First();
        var result = await _service.LogoutAsync(Guid.Parse(user.Id));

        Assert.Equal(ErrorType.INTERNAL_ERROR, result.ErrorType);
    }

    [Fact]
    public async Task Should_DeleteRefreshToken_WhenLogoutWasSuccessful()
    {
        SetupLogout();

        var user = _userDatabase.First();
        var result = await _service.LogoutAsync(Guid.Parse(user.Id));

        Assert.Equal(ErrorType.NONE, result.ErrorType);
        Assert.True(result.Value);
        Assert.Empty(_refreshTokenDatabase);
    }

    [Fact]
    public async Task Should_FailSendingResetPasswordToken_WhenAccountDoesNotExist()
    {
        SetupSendResetPasswordToken();

        var result = await _service.SendResetPasswordTokenAsync("invalidEmail");

        Assert.False(result.HasValue);
        Assert.Equal(ErrorType.BAD_REQUEST, result.ErrorType);
    }

    [Fact]
    public async Task Should_ReturnNothing_WhenSendResetPasswordTokenWasSuccessful()
    {
        var account = GetUserAccount();
        SetupSendResetPasswordToken();

        var result = await _service.SendResetPasswordTokenAsync(account.Email!);

        Assert.Equal(ErrorType.NONE, result.ErrorType);
    }

    [Fact]
    public async Task Should_ContainLinkWithToken_WhenSendResetPasswordTokenWasSuccessful()
    {
        var account = GetUserAccount();
        var expected = GetEmptyEmailPayload();
        var token = GetResetPasswordToken();
        SetupSendResetPasswordToken();
        _emailProvider
            .Setup(obj => obj.Send(It.IsAny<EmailPayload>()))
            .Callback(
                (EmailPayload payload) =>
                {
                    expected = payload;
                }
            );

        var result = await _service.SendResetPasswordTokenAsync(account.Email!);

        Assert.Equal(ErrorType.NONE, result.ErrorType);
        Assert.Contains(token, expected.Body);
    }

    [Fact]
    public async Task Should_SendEmailToCorrectUser_WhenSendResetPasswordTokenWasSuccessful()
    {
        var account = GetUserAccount();
        var expected = GetEmptyEmailPayload();
        SetupSendResetPasswordToken();
        _emailProvider
            .Setup(obj => obj.Send(It.IsAny<EmailPayload>()))
            .Callback(
                (EmailPayload payload) =>
                {
                    expected = payload;
                }
            );

        var result = await _service.SendResetPasswordTokenAsync(account.Email!);

        Assert.Equal(ErrorType.NONE, result.ErrorType);
        Assert.Equal(account.Email!, expected.To.First());
    }

    [Fact]
    public async Task Should_UseSmtpSettings_WhenSendResetPasswordTokenWasSuccessful()
    {
        var account = GetUserAccount();
        var expected = GetEmptyEmailPayload();
        var settings = GetSmtpSettings();
        var token = GetResetPasswordToken();
        SetupSendResetPasswordToken();
        _emailProvider
            .Setup(obj => obj.Send(It.IsAny<EmailPayload>()))
            .Callback(
                (EmailPayload payload) =>
                {
                    expected = payload;
                }
            );

        var result = await _service.SendResetPasswordTokenAsync(account.Email!);

        Assert.Equal(ErrorType.NONE, result.ErrorType);
        Assert.Equal(settings.Host, expected.Host);
        Assert.Equal(settings.Port, expected.Port);
        Assert.Equal(settings.Username, expected.Username);
        Assert.Equal(settings.Password, expected.Password);
        Assert.Equal(settings.Address, expected.From);
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenAccountDoesNotExist_ReturnBadRequestError()
    {
        SetupResetPasswordToken();
        var dto = new ResetPasswordDTO
        {
            Email = "wrongEmail",
            NewPassword = "1234",
            Token = "token",
        };

        var result = await _service.ResetPasswordAsync(dto);

        Assert.Equal(ErrorType.BAD_REQUEST, result.ErrorType);
        Assert.False(result.HasValue);
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenTokenIsInvalid_ReturnForbiddenError()
    {
        SetupResetPasswordToken();
        var account = GetUserAccount();
        var dto = new ResetPasswordDTO
        {
            Email = account.Email!,
            NewPassword = "1234",
            Token = "token",
        };
        var error = new IdentityError { Code = "InvalidToken", Description = "" };
        _accountCollection
            .Setup(obj =>
                obj.ResetPasswordAsync(
                    It.IsAny<UserAccount>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(IdentityResult.Failed([error]));

        var result = await _service.ResetPasswordAsync(dto);

        Assert.Equal(ErrorType.FORBIDDEN, result.ErrorType);
        Assert.False(result.HasValue);
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenPasswordDoesNotFulfillRequirements_ReturnBadRequestError()
    {
        SetupResetPasswordToken();
        var account = GetUserAccount();
        var dto = new ResetPasswordDTO
        {
            Email = account.Email!,
            NewPassword = "1234",
            Token = "token",
        };
        var error = new IdentityError { Code = "OtherCode", Description = "" };
        _accountCollection
            .Setup(obj =>
                obj.ResetPasswordAsync(
                    It.IsAny<UserAccount>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(IdentityResult.Failed([error]));

        var result = await _service.ResetPasswordAsync(dto);

        Assert.Equal(ErrorType.BAD_REQUEST, result.ErrorType);
        Assert.False(result.HasValue);
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenResetPasswordSucceeded_ReturnOk()
    {
        SetupResetPasswordToken();
        var account = GetUserAccount();
        var dto = new ResetPasswordDTO
        {
            Email = account.Email!,
            NewPassword = "1234",
            Token = "token",
        };

        var result = await _service.ResetPasswordAsync(dto);

        Assert.Equal(ErrorType.NONE, result.ErrorType);
        Assert.True(result.HasValue);
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenResetPasswordSucceeded_UpdateUserAccount()
    {
        SetupResetPasswordToken();
        var account = GetUserAccount();
        var dto = new ResetPasswordDTO
        {
            Email = account.Email!,
            NewPassword = "1234",
            Token = "token",
        };

        var result = await _service.ResetPasswordAsync(dto);
        var item = _userDatabase.First();

        Assert.Equal(ErrorType.NONE, result.ErrorType);
        Assert.Equal("1234", item.PasswordHash);
    }
}
