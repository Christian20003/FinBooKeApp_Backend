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
    private readonly ILogger<AuthenticationService> _logger = logger;

    public async Task<Result<User, AuthenticationServiceResultCode>> LoginAsync(
        string email,
        string password
    )
    {
        LogLogin(email);
        var hashedEmail = _hashProvider.Hash(email);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.EmailHash! == hashedEmail
        );
        if (user is null)
        {
            LogInvalidCredentials(email);
            return Result.Error<User, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.INVALID_CREDENTIALS
            );
        }
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, true);
        if (!signInResult.Succeeded)
            return GetSignInFailureResult(signInResult, email);

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
            return Result.Error<User, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.INTERNAL_ERROR
            );
        }

        var result = GetUser(user, accessToken, refreshToken);
        LogLoginSuccess(email);
        return Result.Ok<User, AuthenticationServiceResultCode>(result);
    }

    public async Task<Result<User, AuthenticationServiceResultCode>> RegisterAsync(
        string email,
        string username,
        string password
    )
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
            LogInvalidCredentials(email);
            return Result.Error<User, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.ACCOUNT_CREATION_FAILED
            );
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
            return Result.Error<User, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.INTERNAL_ERROR
            );
        }
        var userDTO = GetUser(user, accessToken, refreshToken);
        LogRegisterSuccess(email);
        return Result.Ok<User, AuthenticationServiceResultCode>(userDTO);
    }

    public async Task<Result<bool, AuthenticationServiceResultCode>> LogoutAsync(string userId)
    {
        LogLogout(userId);
        var user = await _accountCollection.GetAccountAsync(account => account.Id == userId);
        if (user is null)
        {
            LogInvalidUserId(userId);
            return Result.Error<bool, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.USER_NOT_FOUND
            );
        }
        var tokenResult = await _accountCollection.DeleteAccountRefreshTokenAsync(user);
        if (!tokenResult.Succeeded)
        {
            var messages = tokenResult.Errors.Select(error => error.Description).ToList();
            LogInternalError(messages);
            return Result.Error<bool, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.INTERNAL_ERROR
            );
        }
        LogLogoutSuccess(user.EmailHash);
        return Result.Ok<bool, AuthenticationServiceResultCode>(true);
    }

    public async Task<Result<bool, AuthenticationServiceResultCode>> SendResetPasswordTokenAsync(
        string email
    )
    {
        var emailHash = _hashProvider.Hash(email);
        LogSendResetPwdToken(emailHash);

        var user = await _accountCollection.GetAccountAsync(account =>
            account.EmailHash! == emailHash
        );
        if (user is null)
        {
            LogInvalidCredentials(emailHash);
            return Result.Error<bool, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.INVALID_EMAIL
            );
        }
        var token = await _accountCollection.GeneratePasswordResetTokenAsync(user);
        var link = _authenticationSettings.Value.ResetPasswordLink;
        link += $"?token={token}&email={email}";
        var template = _emailTemplateBuilder.GetResetPasswordTemplate(link);
        var subject = EmailText.ResetPasswordEmailSubject;
        var emailPayload = GetEmailPayload(template, email, subject);
        _emailProvider.Send(emailPayload);
        LogResetPasswordTokenSuccess(emailHash);
        return Result.Ok<bool, AuthenticationServiceResultCode>(true);
    }

    public async Task<Result<bool, AuthenticationServiceResultCode>> ResetPasswordAsync(
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
            return Result.Error<bool, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.INVALID_EMAIL
            );
        }
        var resetResult = await _accountCollection.ResetPasswordAsync(user, token, newPassword);
        if (!resetResult.Succeeded)
        {
            LogInvalidCredentials(email);
            var errorCode =
                resetResult.Errors.FirstOrDefault()?.Code == "InvalidToken"
                    ? AuthenticationServiceResultCode.PASSWORD_RESET_TOKEN_INVALID
                    : AuthenticationServiceResultCode.PASSWORD_POLICY_FAILED;
            return Result.Error<bool, AuthenticationServiceResultCode>(errorCode);
        }
        LogResetPasswordSuccess(emailHash);
        return Result.Ok<bool, AuthenticationServiceResultCode>(true);
    }

    public async Task<Result<Session, AuthenticationServiceResultCode>> RefreshAccessTokenAsync(
        string email,
        string refreshToken
    )
    {
        var emailHash = _hashProvider.Hash(email);
        LogRefreshAccessToken(emailHash);
        var user = await _accountCollection.GetAccountAsync(account =>
            account.EmailHash == emailHash
        );
        if (user is null)
        {
            LogInvalidUserId(emailHash);
            return Result.Error<Session, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.USER_NOT_FOUND
            );
        }
        var accountToken = await _accountCollection.GetAccountRefreshTokenAsync(user);
        if (accountToken is null)
        {
            LogMissingRefreshToken(emailHash);
            return Result.Error<Session, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.REFRESH_TOKEN_NOT_FOUND
            );
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
            return Result.Error<Session, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.REFRESH_TOKEN_INVALID
            );
        }
        var accessToken = GetAccessToken(claim.Claims);
        LogRefreshAccessTokenSuccess(emailHash);
        return Result.Ok<Session, AuthenticationServiceResultCode>(
            new Session
            {
                AccessToken = accessToken.Value,
                AccessTokenExpire = accessToken.Expires,
                RefreshToken = refreshToken,
                RefreshTokenExpire = _claimProvider.GetExpires(claim).Ticks,
            }
        );
    }

    private Result<User, AuthenticationServiceResultCode> GetSignInFailureResult(
        SignInResult signInResult,
        string email
    )
    {
        if (signInResult.IsLockedOut)
        {
            LogAccountLock(email);
            return Result.Error<User, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.ACCOUNT_LOCKED
            );
        }

        LogInvalidCredentials(email);
        if (signInResult.IsNotAllowed)
            return Result.Error<User, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.SIGN_IN_NOT_ALLOWED
            );
        if (signInResult.RequiresTwoFactor)
            return Result.Error<User, AuthenticationServiceResultCode>(
                AuthenticationServiceResultCode.TWO_FACTOR_REQUIRED
            );
        return Result.Error<User, AuthenticationServiceResultCode>(
            AuthenticationServiceResultCode.INVALID_CREDENTIALS
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
