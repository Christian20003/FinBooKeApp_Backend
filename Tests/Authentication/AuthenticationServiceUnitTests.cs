using FinBookeAPI.Models.Authentication;
using FinBookeAPI.Models.Token;
using FinBookeAPI.Models.Wrapper;
using FinBookeAPI.Services.Authentication;
using FinBookeAPI.Services.Email;
using FinBookeAPI.Services.SecurityUtility;
using FinBookeAPI.Services.Token;
using FinBookeAPI.Tests.Mocks.Manager;
using FinBookeAPI.Tests.Mocks.Services;
using FinBookeAPI.Tests.Records;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace FinBookeAPI.Tests.Authentication;

public partial class AuthenticationServiceUnitTests
{
    private readonly Mock<IAccountManager> _userManager;
    private readonly Mock<SignInManager<UserAccount>> _signInManager;
    private readonly Mock<ISecurityUtilityService> _securityUtilityService;
    private readonly Mock<ITokenService> _tokenService;
    private readonly Mock<IEmailService> _emailService;
    private readonly Mock<IDataProtection> _dataProtection;
    private readonly Mock<ILogger<AuthenticationService>> _logger;
    private readonly AuthenticationService _service;

    private readonly UserAccount _userAccount;
    private readonly JwtToken _token;

    public AuthenticationServiceUnitTests()
    {
        // Initialize important data objects
        _userAccount = UserAccountRecord.GetObject();
        _token = JwtTokenRecord.GetObject();

        // Initialize dependencies
        _userManager = new Mock<IAccountManager>();
        _signInManager = MockSignInManager.GetMock(MockUserManager.GetMock<UserAccount>());
        _tokenService = MockITokenService.GetMock(_userAccount.Id);
        _securityUtilityService = MockISecurityUtilityService.GetMock();
        _emailService = MockIEmailService.GetMock();
        _dataProtection = MockDataProtection.GetMock();
        _logger = new Mock<ILogger<AuthenticationService>>();

        // Mocking methods that have in most cases the same output
        var accounts = new List<UserAccount> { _userAccount }.AsQueryable();
        _userManager.Setup(obj => obj.GetUsersAsync()).Returns(accounts.ToAsyncEnumerable());
        _userManager
            .Setup(obj => obj.CreateUserAsync(It.IsAny<UserAccount>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Initialize test object
        _service = new AuthenticationService(
            _userManager.Object,
            _signInManager.Object,
            _securityUtilityService.Object,
            _tokenService.Object,
            _emailService.Object,
            _dataProtection.Object,
            _logger.Object
        );
    }
}
