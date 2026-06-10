namespace FinBookeAPI.Services.Authentication;

public partial class AuthenticationService : IAuthenticationService
{
    private static readonly long ACCESS_TOKEN_LIFETIME = 600; // seconds
    private static readonly long REFRESH_TOKEN_LIFETIME = 86400; // seconds

    private static readonly string INVALID_CREDENTIALS_KEY = "InvalidCredentials";
    private static readonly string ACCOUNT_LOCKED_KEY = "AccountLocked";
    private static readonly string INTERNAL_ERROR_KEY = "InternalError";
    private static readonly string RESOURCE_LOCKED = "ResourceLocked";
}
