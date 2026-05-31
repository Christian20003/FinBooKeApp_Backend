using FinBooKeAPI.Collections.AccountCollection;
using FinBooKeAPI.Logic.Authentication;
using FinBooKeAPI.Logic.Email;
using FinBooKeAPI.Logic.Security;
using FinBooKeAPI.Mapping.Authentication;
using FinBookeAPI.Models.Database.Authentication;
using FinBooKeAPI.Models.DTO.Authentication;
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
    IEmailProvider emailProvider,
    IEmailTemplateBuilder emailTemplateBuilder,
    IOptions<AuthenticationSettings> authenticationSettings,
    IOptions<SmtpSettings> smtpSettings,
    IStringLocalizer<AuthenticationService> localizer,
    ILogger<AuthenticationService> logger
) : IAuthenticationService
{
    private const long ACCESS_TOKEN_LIFETIME = 600; // seconds
    private const long REFRESH_TOKEN_LIFETIME = 86400; // seconds

    private readonly SignInManager<UserAccount> _signInManager = signInManager;
    private readonly IAccountCollection _accountCollection = accountCollection;
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IClaimProvider _claimProvider = claimProvider;
    private readonly IDataProtection _protection = protection;
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
        var user = await _accountCollection.GetAccountAsync(account =>
            _protection.Unprotect(account.Email!) == loginData.Email
        );
        if (user is null)
        {
            LogInvalidCredentials(loginData.Email);
            return Result.BadRequest<UserDTO>(_localizer.GetString("InvalidCredentials"));
        }
        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            loginData.Password,
            true
        );
        if (signInResult.IsLockedOut)
        {
            LogAccountLock(loginData.Email);
            return Result.Forbidden<UserDTO>(_localizer.GetString("AccountLocked"));
        }
        if (!signInResult.Succeeded)
        {
            LogInvalidCredentials(loginData.Email);
            return Result.BadRequest<UserDTO>(_localizer.GetString("InvalidCredentials"));
        }

        var claims = _claimProvider.CreateClaims(user.Id, _protection.Unprotect(user.Email!));
        var expirationAccessToken = DateTime.UtcNow.AddSeconds(ACCESS_TOKEN_LIFETIME);
        var expirationRefreshToken = DateTime.UtcNow.AddSeconds(REFRESH_TOKEN_LIFETIME);
        var accessTokenPayload = TokenMapper.GetAccessTokenCreateTokenPayload(
            claims,
            expirationAccessToken,
            _authenticationSettings
        );
        var refreshTokenPayload = TokenMapper.GetRefreshTokenCreateTokenPayload(
            claims,
            expirationRefreshToken,
            _authenticationSettings
        );
        var accessToken = _tokenProvider.CreateToken(accessTokenPayload);
        var refreshToken = _tokenProvider.CreateToken(refreshTokenPayload);
        var tokenResult = await _accountCollection.SetAccountRefreshTokenAsync(
            user,
            refreshToken.Value
        );
        if (!tokenResult.Succeeded)
        {
            var messages = tokenResult.Errors.Select(error => error.Description).ToList();
            LogInternalError(messages);
            return Result.InternalError<UserDTO>(messages);
        }

        var userDTO = UserMapper.GetUserDTO(user, accessToken, refreshToken, _protection);
        LogLoginSuccess(loginData.Email);
        return Result.Ok(userDTO);
    }
}
