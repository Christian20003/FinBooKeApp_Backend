using System.Security.Claims;
using FinBooKeAPI.Collections.AccountCollection;
using FinBooKeAPI.Logic.Authentication;
using FinBooKeAPI.Logic.Email;
using FinBooKeAPI.Logic.Security;
using FinBooKeAPI.Mapping.Authentication;
using FinBookeAPI.Models.Database.Authentication;
using FinBooKeAPI.Models.DTO.Authentication;
using FinBooKeAPI.Models.Logic.Authentication;
using FinBookeAPI.Models.Result;
using FinBooKeAPI.Models.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService(
    SignInManager<UserAccount> signInManager,
    IAccountCollection accountCollection,
    ITokenProvider tokenProvider,
    IClaimProvider claimProvider,
    IDataProtection protection,
    IHashProvider hashProvider,
    IEmailProvider emailProvider,
    IEmailTemplateBuilder emailTemplateBuilder,
    IOptions<AuthenticationSettings> authenticationSettings,
    IOptions<SmtpSettings> smtpSettings,
    IStringLocalizer<AuthenticationService> localizer,
    ILogger<AuthenticationService> logger
) : IAuthenticationService
{
    private readonly SignInManager<UserAccount> _signInManager = signInManager;
    private readonly IAccountCollection _accountCollection = accountCollection;
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IClaimProvider _claimProvider = claimProvider;
    private readonly IDataProtection _protection = protection;
    private readonly IHashProvider _hashProvider = hashProvider;
    private readonly IEmailProvider _emailProvider = emailProvider;
    private readonly IEmailTemplateBuilder _emailTemplateBuilder = emailTemplateBuilder;
    private readonly IOptions<AuthenticationSettings> _authenticationSettings =
        authenticationSettings;
    private readonly IOptions<SmtpSettings> _smtpSettings = smtpSettings;
    private readonly IStringLocalizer<AuthenticationService> _localizer = localizer;
    private readonly ILogger<AuthenticationService> _logger = logger;

    public async Task<Result<UserDTO>> LoginAsync(LoginDTO loginData)
    {
        LogLogin(loginData.Email);
        var hashedEmail = _hashProvider.Hash(loginData.Email);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.EmailHash! == hashedEmail
        );
        if (user is null)
        {
            LogInvalidCredentials(loginData.Email);
            return Result.BadRequest<UserDTO>(_localizer.GetString(INVALID_CREDENTIALS_KEY));
        }
        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            loginData.Password,
            true
        );
        if (signInResult.IsLockedOut)
        {
            LogAccountLock(loginData.Email);
            return Result.Forbidden<UserDTO>(_localizer.GetString(ACCOUNT_LOCKED_KEY));
        }
        if (!signInResult.Succeeded)
        {
            LogInvalidCredentials(loginData.Email);
            return Result.BadRequest<UserDTO>(_localizer.GetString(INVALID_CREDENTIALS_KEY));
        }

        var claims = _claimProvider.CreateClaims(user.Id, _protection.Unprotect(user.Email!));
        var accessToken = GetAccessToken(claims);
        var refreshToken = GetRefreshToken(claims);
        var tokenResult = await _accountCollection.SetAccountRefreshTokenAsync(
            user,
            refreshToken.Value
        );
        if (!tokenResult.Succeeded)
        {
            var messages = tokenResult.Errors.Select(error => error.Description).ToList();
            LogInternalError(messages);
            return Result.InternalError<UserDTO>(_localizer.GetString(INTERNAL_ERROR_KEY));
        }

        var userDTO = UserMapper.GetUserDTO(user, accessToken, refreshToken, _protection);
        LogLoginSuccess(loginData.Email);
        return Result.Ok(userDTO);
    }

    public async Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerData)
    {
        LogRegister(registerData.Email);
        var user = new UserAccount
        {
            UserName = registerData.Username,
            Email = _protection.Protect(registerData.Email),
            EmailHash = _hashProvider.Hash(registerData.Email),
        };
        var registerResult = await _accountCollection.CreateAccountAsync(
            user,
            registerData.Password
        );
        if (!registerResult.Succeeded)
        {
            var messages = registerResult.Errors.Select(error => error.Description).ToList();
            LogInvalidCredentials(registerData.Email);
            return Result.BadRequest<UserDTO>(messages);
        }
        var claims = _claimProvider.CreateClaims(user.Id, _protection.Unprotect(user.Email!));
        var accessToken = GetAccessToken(claims);
        var refreshToken = GetRefreshToken(claims);
        var tokenResult = await _accountCollection.SetAccountRefreshTokenAsync(
            user,
            refreshToken.Value
        );
        if (!tokenResult.Succeeded)
        {
            var messages = tokenResult.Errors.Select(error => error.Description).ToList();
            LogInternalError(messages);
            return Result.InternalError<UserDTO>(_localizer.GetString(INTERNAL_ERROR_KEY));
        }
        var userDTO = UserMapper.GetUserDTO(user, accessToken, refreshToken, _protection);
        LogRegisterSuccess(registerData.Email);
        return Result.Ok(userDTO);
    }

    private AuthenticationToken GetAccessToken(IEnumerable<Claim> claims)
    {
        var expirationAccessToken = DateTime.UtcNow.AddSeconds(ACCESS_TOKEN_LIFETIME);
        var accessTokenPayload = TokenMapper.GetAccessTokenCreateTokenPayload(
            claims,
            expirationAccessToken,
            _authenticationSettings
        );
        return _tokenProvider.CreateToken(accessTokenPayload);
    }

    private AuthenticationToken GetRefreshToken(IEnumerable<Claim> claims)
    {
        var expirationRefreshToken = DateTime.UtcNow.AddSeconds(REFRESH_TOKEN_LIFETIME);
        var refreshTokenPayload = TokenMapper.GetRefreshTokenCreateTokenPayload(
            claims,
            expirationRefreshToken,
            _authenticationSettings
        );
        return _tokenProvider.CreateToken(refreshTokenPayload);
    }
}
