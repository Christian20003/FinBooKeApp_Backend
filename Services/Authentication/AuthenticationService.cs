using FinBookeAPI.Models.Authentication;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Wrapper;
using FinBookeAPI.Services.Email;
using FinBookeAPI.Services.SecurityUtility;
using FinBookeAPI.Services.Token;
using Microsoft.AspNetCore.Identity;

namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService(
    IAccountManager accountManager,
    SignInManager<UserAccount> signInManager,
    ISecurityUtilityService securityUtilityService,
    ITokenService tokenService,
    IEmailService emailService,
    IDataProtection protection,
    ILogger<AuthenticationService> logger
) : IAuthenticationService
{
    private readonly IAccountManager _accountManager = accountManager;
    private readonly SignInManager<UserAccount> _signInManager = signInManager;
    private readonly ISecurityUtilityService _securityUtilityService = securityUtilityService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IEmailService _emailService = emailService;
    private readonly IDataProtection _protector = protection;
    private readonly ILogger<AuthenticationService> _logger = logger;

    [LoggerMessage(
        EventId = LogEvents.ConfigurationError,
        Level = LogLevel.Critical,
        Message = "{Type}: {Message} - {Trace}"
    )]
    private partial void LogConfigurationError(string type, string message, string? trace);

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidCredentials,
        Level = LogLevel.Error,
        Message = "Authentication: Invalid email - {Email}"
    )]
    private partial void LogInvalidEmail(string email);
}
