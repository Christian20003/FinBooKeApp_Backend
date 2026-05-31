using System.Linq.Expressions;
using FinBooKeAPI.Collections.AccountCollection;
using FinBooKeAPI.Logic.Authentication;
using FinBooKeAPI.Logic.Email;
using FinBooKeAPI.Logic.Security;
using FinBookeAPI.Models.Database.Authentication;
using FinBooKeAPI.Models.DTO.Authentication;
using FinBooKeAPI.Models.Logic.Authentication;
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
    private readonly Mock<IEmailProvider> _emailProvider;
    private readonly Mock<IEmailTemplateBuilder> _emailTemplateBuilder;
    private readonly Mock<IOptions<AuthenticationSettings>> _authenticationSettings;
    private readonly Mock<IOptions<SmtpSettings>> _smtpSettings;
    private readonly Mock<IStringLocalizer<AuthenticationService>> _localizer;
    private readonly Mock<ILogger<AuthenticationService>> _logger;

    private readonly AuthenticationService _service;

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
        _emailProvider = new Mock<IEmailProvider>();
        _emailTemplateBuilder = new Mock<IEmailTemplateBuilder>();
        _authenticationSettings = new Mock<IOptions<AuthenticationSettings>>();
        _smtpSettings = new Mock<IOptions<SmtpSettings>>();
        _localizer = new Mock<IStringLocalizer<AuthenticationService>>();
        _logger = new Mock<ILogger<AuthenticationService>>();

        _authenticationSettings.Setup(obj => obj.Value).Returns(GetAuthenticationSettings());

        _service = new AuthenticationService(
            _signInManager.Object,
            _accountCollection.Object,
            _tokenProvider.Object,
            _claimProvider.Object,
            _protection.Object,
            _emailProvider.Object,
            _emailTemplateBuilder.Object,
            _authenticationSettings.Object,
            _smtpSettings.Object,
            _localizer.Object,
            _logger.Object
        );
    }

    public static LoginDTO GetLoginDTO()
    {
        return new LoginDTO { Email = "email", Password = "password" };
    }

    public static UserAccount GetUserAccount()
    {
        return new UserAccount
        {
            Id = "id",
            UserName = "name",
            Email = "email",
            ImagePath = "path",
            PasswordHash = "hash",
        };
    }

    public static AuthenticationSettings GetAuthenticationSettings()
    {
        return new AuthenticationSettings
        {
            Issuer = "issuer",
            Audience = "audience",
            AccessTokenSecret = "secret",
            RefreshTokenSecret = "secret",
        };
    }

    public static AuthenticationToken GetAuthenticationToken()
    {
        return new AuthenticationToken { Value = "token", Expires = DateTime.UtcNow.Ticks };
    }

    [Fact]
    public async Task Should_FailLogin_WhenEmailIsInvalid()
    {
        var loginData = GetLoginDTO();
        _accountCollection
            .Setup(obj => obj.GetAccountAsync(It.IsAny<Expression<Func<UserAccount, bool>>>()))
            .ReturnsAsync((UserAccount?)null);

        var result = await _service.LoginAsync(loginData);

        Assert.Equal(ErrorType.BAD_REQUEST, result.ErrorType);
    }

    [Fact]
    public async Task Should_FailLogin_WhenAccountIsLocked()
    {
        var account = GetUserAccount();
        var loginData = GetLoginDTO();
        _accountCollection
            .Setup(obj => obj.GetAccountAsync(It.IsAny<Expression<Func<UserAccount, bool>>>()))
            .ReturnsAsync(account);
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
        var account = GetUserAccount();
        var loginData = GetLoginDTO();
        _accountCollection
            .Setup(obj => obj.GetAccountAsync(It.IsAny<Expression<Func<UserAccount, bool>>>()))
            .ReturnsAsync(account);
        _signInManager
            .Setup(obj =>
                obj.CheckPasswordSignInAsync(
                    It.IsAny<UserAccount>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()
                )
            )
            .ReturnsAsync(SignInResult.Failed);

        var result = await _service.LoginAsync(loginData);

        Assert.Equal(ErrorType.BAD_REQUEST, result.ErrorType);
    }

    [Fact]
    public async Task Should_FailLogin_WhenRefreshTokenCouldNotBeStored()
    {
        var token = GetAuthenticationToken();
        var account = GetUserAccount();
        var loginData = GetLoginDTO();
        _accountCollection
            .Setup(obj => obj.GetAccountAsync(It.IsAny<Expression<Func<UserAccount, bool>>>()))
            .ReturnsAsync(account);
        _accountCollection
            .Setup(obj =>
                obj.SetAccountRefreshTokenAsync(It.IsAny<UserAccount>(), It.IsAny<string>())
            )
            .ReturnsAsync(IdentityResult.Failed([]));
        _signInManager
            .Setup(obj =>
                obj.CheckPasswordSignInAsync(
                    It.IsAny<UserAccount>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()
                )
            )
            .ReturnsAsync(SignInResult.Success);
        _tokenProvider.Setup(obj => obj.CreateToken(It.IsAny<CreateTokenPayload>())).Returns(token);

        var result = await _service.LoginAsync(loginData);

        Assert.Equal(ErrorType.INTERNAL_ERROR, result.ErrorType);
    }

    [Fact]
    public async Task Should_SucceedLogin_WhenCredentialsAreValid()
    {
        var token = GetAuthenticationToken();
        var account = GetUserAccount();
        var loginData = GetLoginDTO();
        _accountCollection
            .Setup(obj => obj.GetAccountAsync(It.IsAny<Expression<Func<UserAccount, bool>>>()))
            .ReturnsAsync(account);
        _accountCollection
            .Setup(obj =>
                obj.SetAccountRefreshTokenAsync(It.IsAny<UserAccount>(), It.IsAny<string>())
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
            .ReturnsAsync(SignInResult.Success);
        _tokenProvider.Setup(obj => obj.CreateToken(It.IsAny<CreateTokenPayload>())).Returns(token);

        var result = await _service.LoginAsync(loginData);

        Assert.Equal(ErrorType.NONE, result.ErrorType);
        Assert.True(result.HasValue);
    }

    [Fact]
    public async Task Should_ReturnCorrectUserAccount_WhenLoginWasSuccessuful()
    {
        var token = GetAuthenticationToken();
        var account = GetUserAccount();
        var loginData = GetLoginDTO();
        _accountCollection
            .Setup(obj => obj.GetAccountAsync(It.IsAny<Expression<Func<UserAccount, bool>>>()))
            .ReturnsAsync(account);
        _accountCollection
            .Setup(obj =>
                obj.SetAccountRefreshTokenAsync(It.IsAny<UserAccount>(), It.IsAny<string>())
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
            .ReturnsAsync(SignInResult.Success);
        _tokenProvider.Setup(obj => obj.CreateToken(It.IsAny<CreateTokenPayload>())).Returns(token);
        _protection
            .Setup(obj => obj.Unprotect(It.IsAny<string>()))
            .Returns<string>(
                (value) =>
                {
                    return value;
                }
            );

        var result = await _service.LoginAsync(loginData);
        var user = result.Value!;

        Assert.Equal(loginData.Email, user.Email);
    }

    [Fact]
    public async Task Should_StoreRefreshToken_WhenLoginWasSuccessuful()
    {
        var token = GetAuthenticationToken();
        var account = GetUserAccount();
        var loginData = GetLoginDTO();
        string? authenticationToken = null;
        _accountCollection
            .Setup(obj => obj.GetAccountAsync(It.IsAny<Expression<Func<UserAccount, bool>>>()))
            .ReturnsAsync(account);
        _accountCollection
            .Setup(obj =>
                obj.SetAccountRefreshTokenAsync(It.IsAny<UserAccount>(), It.IsAny<string>())
            )
            .Callback<UserAccount, string>(
                (account, token) =>
                {
                    authenticationToken = token;
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
            .ReturnsAsync(SignInResult.Success);
        _tokenProvider.Setup(obj => obj.CreateToken(It.IsAny<CreateTokenPayload>())).Returns(token);
        _protection
            .Setup(obj => obj.Unprotect(It.IsAny<string>()))
            .Returns<string>(
                (value) =>
                {
                    return value;
                }
            );

        _ = await _service.LoginAsync(loginData);

        Assert.Equal(token.Value, authenticationToken);
    }
}
