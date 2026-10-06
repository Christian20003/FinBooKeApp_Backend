namespace FinBooKeApp.Api.Shared.Logging;

public static class LogEvents
{
    // 5000 - 5999

    public const int AuthenticationRequest = 5000;
    public const int CategoryRequest = 5010;

    public const int ProfileRequest = 5020;

    public const int UploadPostRequest = 5020;
    public const int UploadGetRequest = 5021;
    public const int UploadDeleteRequest = 5022;

    // 6000 - 6999

    public const int OperationIgnored = 6000;
    public const int ConfigurationError = 6001;

    // TODO
    public const int AmountManagementOperationFailed = 8020;
}
