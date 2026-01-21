namespace FinBookeAPI.Models.Configuration;

/// <summary>
/// This enumeration includes all possible log events.
/// </summary>
public static class LogEvents
{
    public const int AuthenticationRequest = 4000;
    public const int CategoryRequest = 4010;

    public const int OperationIgnored = 5000;

    public const int AuthenticationSuccess = 7000;
    public const int ResetCredentialsSuccess = 7010;
    public const int CategoryOperationSuccess = 7015;
    public const int CategoryUpdateSuccess = 7016;
    public const int CategoryInsertSuccess = 7017;
    public const int CategoryReadSuccess = 7018;
    public const int CategoryDeleteSuccess = 7019;
    public const int PaymentMethodOperationSuccess = 7030;
    public const int PaymentMethodUpdateSuccess = 7031;
    public const int PaymentMethodInsertSuccess = 7032;
    public const int PaymentMethodReadSuccess = 7033;
    public const int PaymentMethodDeleteSuccess = 7034;
    public const int UploadFileSuccess = 7040;

    public const int AuthenticationFailed = 8000;
    public const int AuthorizationFailed = 8005;
    public const int ResetCredentialsFailed = 8010;
    public const int CategoryOperationFailed = 8015;
    public const int CategoryUpdateFailed = 8016;
    public const int CategoryInsertFailed = 8017;
    public const int CategoryReadFailed = 8018;
    public const int CategoryDeleteFailed = 8019;
    public const int AmountManagementOperationFailed = 8020;
    public const int PaymentMethodOperationFailed = 8030;
    public const int PaymentMethodUpdateFailed = 8031;
    public const int PaymentMethodInsertFailed = 8032;
    public const int PaymentMethodReadFailed = 8033;
    public const int PaymentMethodDeleteFailed = 8034;
    public const int UploadFileFailed = 8040;

    public const int ConfigurationError = 9000;
}
