using System.Security.Claims;
using FinBooKeApp.Core.Services.Authentication.Models;
using FinBooKeApp.Core.Shared.Authentication.Interfaces;
using FinBooKeApp.Core.Shared.Authentication.Models;
using FinBooKeApp.Core.Shared.Email.Interfaces;
using FinBooKeApp.Core.Shared.Email.Models;
using FinBookeApp.Core.Shared.Result;
using FinBooKeApp.Core.Shared.Security.Interfaces;
using FinBooKeApp.Data.Interfaces;
using FinBooKeApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinBooKeApp.Core.Services.Authentication;

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

    public async Task<Result<User>> LoginAsync(string email, string password)
    {
        LogLogin(email);
        var hashedEmail = _hashProvider.Hash(email);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.EmailHash! == hashedEmail
        );
        if (user is null)
        {
            LogInvalidCredentials(email);
            return Result.BadRequest<User>(_localizer.GetString(INVALID_CREDENTIALS_KEY));
        }
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, true);
        if (signInResult.IsLockedOut)
        {
            LogAccountLock(email);
            return Result.Forbidden<User>(_localizer.GetString(ACCOUNT_LOCKED_KEY));
        }
        if (!signInResult.Succeeded)
        {
            LogInvalidCredentials(email);
            return Result.BadRequest<User>(_localizer.GetString(INVALID_CREDENTIALS_KEY));
        }

        var claims = _claimProvider.CreateClaims(user.Id, _protection.UnprotectEmail(user.Email!));
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
            return Result.InternalError<User>(_localizer.GetString(INTERNAL_ERROR_KEY));
        }

        var result = GetUser(user, accessToken, refreshToken);
        LogLoginSuccess(email);
        return Result.Ok(result);
    }

    public async Task<Result<User>> RegisterAsync(string email, string username, string password)
    {
        LogRegister(email);
        var user = new UserAccount
        {
            UserName = username,
            Email = _protection.ProtectEmail(email),
            EmailHash = _hashProvider.Hash(email),
        };
        var registerResult = await _accountCollection.CreateAccountAsync(user, password);
        if (!registerResult.Succeeded)
        {
            var messages = registerResult.Errors.Select(error => error.Description).ToList();
            LogInvalidCredentials(email);
            return Result.BadRequest<User>(messages);
        }
        var claims = _claimProvider.CreateClaims(user.Id, _protection.UnprotectEmail(user.Email!));
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
            return Result.InternalError<User>(_localizer.GetString(INTERNAL_ERROR_KEY));
        }
        var userDTO = GetUser(user, accessToken, refreshToken);
        LogRegisterSuccess(email);
        return Result.Ok(userDTO);
    }

    public async Task<Result<bool>> LogoutAsync(string userId)
    {
        LogLogout(userId);
        var user = await _accountCollection.GetAccountAsync(account => account.Id == userId);
        if (user is null)
        {
            LogInvalidUserId(userId);
            return Result.Unauthorized<bool>("");
        }
        var tokenResult = await _accountCollection.DeleteAccountRefreshTokenAsync(user);
        if (!tokenResult.Succeeded)
        {
            var messages = tokenResult.Errors.Select(error => error.Description).ToList();
            LogInternalError(messages);
            return Result.InternalError<bool>(_localizer.GetString(INTERNAL_ERROR_KEY));
        }
        LogLogoutSuccess(user.EmailHash);
        return Result.Ok(true);
    }

    public async Task<Result<bool>> SendResetPasswordTokenAsync(string email)
    {
        var emailHash = _hashProvider.Hash(email);
        LogSendResetPwdToken(emailHash);

        var user = await _accountCollection.GetAccountAsync(account =>
            account.EmailHash! == emailHash
        );
        if (user is null)
        {
            LogInvalidCredentials(emailHash);
            return Result.BadRequest<bool>(_localizer.GetString(INVALID_EMAIL_KEY));
        }
        var token = await _accountCollection.GeneratePasswordResetTokenAsync(user);
        var link = _authenticationSettings.Value.ResetPasswordLink;
        link += $"?token={token}&email={email}";
        var template = _emailTemplateBuilder.GetResetPasswordTemplate(link);
        var subject = _localizer.GetString(RESET_PASSWORD_EMAIL_SUBJECT_KEY);
        var emailPayload = GetEmailPayload(template, email, subject);
        _emailProvider.Send(emailPayload);
        LogResetPasswordTokenSuccess(emailHash);
        return Result.Ok(true);
    }

    public async Task<Result<bool>> ResetPasswordAsync(
        string email,
        string newPassword,
        string token
    )
    {
        var emailHash = _hashProvider.Hash(email);
        LogResetPassword(emailHash);

        var user = await _accountCollection.GetAccountAsync(account =>
            account.EmailHash! == emailHash
        );
        if (user is null)
        {
            LogInvalidCredentials(emailHash);
            return Result.BadRequest<bool>(_localizer.GetString(INVALID_EMAIL_KEY));
        }
        var resetResult = await _accountCollection.ResetPasswordAsync(user, token, newPassword);
        if (!resetResult.Succeeded)
        {
            LogInvalidCredentials(email);
            var messages = resetResult.Errors.Select(error => error.Description).ToList();
            var code = resetResult.Errors.First().Code;
            if (code == "InvalidToken")
                return Result.Forbidden<bool>(messages.First());
            return Result.BadRequest<bool>(messages);
        }
        LogResetPasswordSuccess(emailHash);
        return Result.Ok(true);
    }

    public async Task<Result<Session>> RefreshAccessTokenAsync(string email, string refreshToken)
    {
        var emailHash = _hashProvider.Hash(email);
        LogRefreshAccessToken(emailHash);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.EmailHash == emailHash
        );
        if (user is null)
        {
            LogInvalidUserId(emailHash);
            return Result.Unauthorized<Session>("");
        }
        var accountToken = await _accountCollection.GetAccountRefreshTokenAsync(user);
        if (accountToken is null)
        {
            LogMissingRefreshToken(emailHash);
            return Result.Unauthorized<Session>("");
        }
        var payload = new VerifyTokenPayload
        {
            Issuer = _authenticationSettings.Value.Issuer,
            Audience = _authenticationSettings.Value.Audience,
            Secret = _authenticationSettings.Value.RefreshTokenSecret,
            Token = refreshToken,
        };
        var claim = _tokenProvider.VerifyToken(payload);
        if (accountToken != refreshToken)
        {
            LogInvalidRefreshToken(emailHash);
            return Result.Unauthorized<Session>("");
        }
        var accessToken = GetAccessToken(claim.Claims);
        LogRefreshAccessTokenSuccess(emailHash);
        return Result.Ok(
            new Session
            {
                AccessToken = accessToken.Value,
                AccessTokenExpire = accessToken.Expires,
                RefreshToken = refreshToken,
                RefreshTokenExpire = _claimProvider.GetExpires(claim).Ticks,
            }
        );
    }

    private AuthenticationToken GetAccessToken(IEnumerable<Claim> claims)
    {
        var expirationAccessToken = DateTime.UtcNow.AddSeconds(ACCESS_TOKEN_LIFETIME);
        var accessTokenPayload = GetAccessTokenCreateTokenPayload(claims, expirationAccessToken);
        return _tokenProvider.CreateToken(accessTokenPayload);
    }

    private AuthenticationToken GetRefreshToken(IEnumerable<Claim> claims)
    {
        var expirationRefreshToken = DateTime.UtcNow.AddSeconds(REFRESH_TOKEN_LIFETIME);
        var refreshTokenPayload = GetRefreshTokenCreateTokenPayload(claims, expirationRefreshToken);
        return _tokenProvider.CreateToken(refreshTokenPayload);
    }

    private User GetUser(
        UserAccount user,
        AuthenticationToken accessToken,
        AuthenticationToken refreshToken
    )
    {
        return new User
        {
            Email = _protection.UnprotectEmail(user.Email!),
            Name = user.UserName!,
            ImagePath = user.ImagePath,
            Theme = user.ThemeType,
            Language = user.FrontendLanguage,
            Session = new Session
            {
                AccessToken = accessToken.Value,
                RefreshToken = refreshToken.Value,
                AccessTokenExpire = accessToken.Expires,
                RefreshTokenExpire = refreshToken.Expires,
            },
        };
    }

    private EmailPayload GetEmailPayload(string body, string email, string subject)
    {
        return new EmailPayload
        {
            Host = _smtpSettings.Value.Host,
            Port = _smtpSettings.Value.Port,
            Username = _smtpSettings.Value.Username,
            Password = _smtpSettings.Value.Password,
            From = _smtpSettings.Value.Address,
            To = [email],
            Subject = subject,
            Body = body,
            IsHtml = true,
        };
    }

    private CreateTokenPayload GetAccessTokenCreateTokenPayload(
        IEnumerable<Claim> claims,
        DateTime Expiration
    )
    {
        return new CreateTokenPayload
        {
            Issuer = _authenticationSettings.Value.Issuer,
            Audience = _authenticationSettings.Value.Audience,
            Secret = _authenticationSettings.Value.AccessTokenSecret,
            Expires = Expiration,
            Claims = claims,
        };
    }

    private CreateTokenPayload GetRefreshTokenCreateTokenPayload(
        IEnumerable<Claim> claims,
        DateTime Expiration
    )
    {
        return new CreateTokenPayload
        {
            Issuer = _authenticationSettings.Value.Issuer,
            Audience = _authenticationSettings.Value.Audience,
            Secret = _authenticationSettings.Value.RefreshTokenSecret,
            Expires = Expiration,
            Claims = claims,
        };
    }
}
