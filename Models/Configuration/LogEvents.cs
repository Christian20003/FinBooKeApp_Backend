namespace FinBookeAPI.Models.Configuration;

/// <summary>
/// This enumeration includes all possible log events.
/// </summary>
public static class LogEvents
{
    // 1000 - 1999

    public const int AuthenticationLogin = 1000;
    public const int AuthenticationRegister = 1001;
    public const int AuthenticationLogout = 1002;
    public const int AuthenticationCreateToken = 1003;
    public const int AuthenticationResetPassword = 1004;
    public const int AuthenticationSendAccessCode = 1005;

    public const int EmailSend = 1021;

    public const int UploadFile = 1031;
    public const int UploadGetFile = 1032;

    public const int SecurityCreateAccessCode = 1041;
    public const int SecurityCreatePassword = 1042;
    public const int SecurityHashGeneration = 1043;
    public const int SecurityHashVerification = 1044;

    public const int TokenCleanDatabase = 1051;
    public const int TokenCreateAccessToken = 1052;
    public const int TokenCreateRefreshToken = 1053;
    public const int TokenStoreAccessToken = 1054;
    public const int TokenStoreRefreshToken = 1055;
    public const int TokenCheckExists = 1056;
    public const int TokenVerifyAccessToken = 1057;
    public const int TokenVerifyRefreshToken = 1058;

    public const int CategoryCreate = 1071;
    public const int CategorUpdate = 1072;
    public const int CategoryDelete = 1073;
    public const int CategoryRead = 1074;
    public const int CategoryReadAll = 1075;
    public const int CategoryNest = 1076;

    public const int PaymentMethodCreate = 1091;
    public const int PaymentMethodUpdate = 1092;
    public const int PaymentMethodDelete = 1093;
    public const int PaymentMethodRead = 1094;

    // 2000 - 2999

    public const int AuthenticationSucceededLogin = 2000;
    public const int AuthenticationSucceededRegister = 2001;
    public const int AuthenticationSucceededLogout = 2002;
    public const int AuthenticationTokenCreated = 2003;
    public const int AuthenticationSucceededResetPassword = 2004;
    public const int AuthenticationSucceededSendAccessCode = 2005;

    public const int EmailSendSuccess = 2021;

    public const int UploadFileSuccess = 2031;
    public const int UploadGetFileSuccess = 2032;

    public const int SecurityCreateAccessCodeSuccess = 2041;
    public const int SecurityCreatePasswordSuccess = 2042;
    public const int SecurityHashGenerationSuccess = 2043;
    public const int SecurityHashVerificationSuccess = 2044;

    public const int TokenCreateAccessTokenSuccess = 2052;
    public const int TokenCreateRefreshTokenSuccess = 2053;
    public const int TokenStoreAccessTokenSuccess = 2054;
    public const int TokenStoreRefreshTokenSuccess = 2055;
    public const int TokenVerifyAccessTokenSuccess = 2056;
    public const int TokenVerifyRefreshTokenSuccess = 2057;

    public const int CategoryCreateSuccess = 2071;
    public const int CategorUpdateSuccess = 2072;
    public const int CategoryDeleteSuccess = 2073;
    public const int CategoryReadSuccess = 2074;
    public const int CategoryReadAllSuccess = 2075;
    public const int CategoryNestSuccess = 2076;

    public const int PaymentMethodCreateSuccess = 2091;
    public const int PaymentMethodUpdateSuccess = 2092;
    public const int PaymentMethodDeleteSuccess = 2093;
    public const int PaymentMethodReadSuccess = 2094;

    // 4000 - 4999

    public const int AuthenticationInvalidCredentials = 4000;
    public const int AuthenticationInvalidUserId = 4001;
    public const int AuthenticationInvalidUsername = 4002;
    public const int AuthenticationInvalidToken = 4003;
    public const int AuthenticationInvalidAccessCode = 4004;
    public const int AuthenticationLockedAccount = 4005;
    public const int AuthenticationExpiredAccessCode = 4006;
    public const int AuthenticationRevokedAccount = 4007;

    public const int EmailInvalidHost = 4021;
    public const int EmailInvalidPort = 4022;
    public const int EmailInvalidSender = 4023;
    public const int EmailInvalidEmail = 4024;

    public const int UploadInvalidFileFormat = 4031;
    public const int UploadInvalidFileSize = 4032;
    public const int UploadFileDoesNotExist = 4033;
    public const int UploadInvalidFileName = 4034;

    public const int SecurityInvalidLength = 4041;
    public const int SecurityInvalidHash = 4042;

    public const int TokenInvalidIssuer = 4051;
    public const int TokenInvalidAudience = 4052;
    public const int TokenInvalidSecret = 4053;
    public const int TokenInvalidAccessTokenLifetime = 4054;
    public const int TokenInvalidRefreshTokenLifetime = 4055;

    public const int CategoryDuplicate = 4071;
    public const int CategoryInvalidId = 4072;
    public const int CategoryNotFound = 4073;
    public const int CategoryNotAccessible = 4074;
    public const int CategoryInvalidName = 4075;
    public const int CategoryInvalidColor = 4076;
    public const int CategoryInvalidLimit = 4077;
    public const int CategoryInvalidChild = 4078;

    public const int PaymentNotFound = 4091;
    public const int PaymentNotAccessible = 4092;
    public const int PaymentMethodDuplicate = 4093;

    // 5000 - 5999

    public const int AuthenticationRequest = 5000;
    public const int CategoryRequest = 5010;

    // 6000 - 6999

    public const int OperationIgnored = 6000;
    public const int ConfigurationError = 6001;

    // TODO
    public const int AmountManagementOperationFailed = 8020;
}
