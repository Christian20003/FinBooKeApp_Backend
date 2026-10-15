namespace FinBooKeApp.Core.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    private static readonly long ACCESS_TOKEN_LIFETIME = 600; // seconds
    private static readonly long REFRESH_TOKEN_LIFETIME = 86400; // seconds
}
