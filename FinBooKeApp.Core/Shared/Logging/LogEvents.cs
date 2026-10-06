namespace FinBooKeApp.Core.Shared.Logging;

public static class LogEvents
{
    // 1000 - 1999

    public const int AuthenticationLogin = 1000;
    public const int AuthenticationRegister = 1001;
    public const int AuthenticationLogout = 1002;
    public const int AuthenticationRefreshAccessToken = 1003;
    public const int AuthenticationResetPassword = 1004;
    public const int AuthenticationSendResetPasswordToken = 1005;

    public const int DataImportUser = 1010;

    public const int ProfileChangeEmailToken = 1020;
    public const int ProfileChangeEmail = 1021;
    public const int ProfileDeleteImage = 1022;
    public const int ProfileVerifyEmailToken = 1023;
    public const int ProfileSetProfileImage = 1024;
    public const int ProfileSetProfileLanguage = 1025;
    public const int ProfileSetProfileTheme = 1026;
    public const int ProfileSetUsername = 1027;
    public const int ProfileVerifyEmail = 1028;
    public const int ProfileGetProfileImage = 1029;

    // 2000 - 2999

    public const int AuthenticationLoginSuccess = 2000;
    public const int AuthenticationRegisterSuccess = 2001;
    public const int AuthenticationLogoutSuccess = 2002;
    public const int AuthenticationResetPasswordTokenSuccess = 2003;
    public const int AuthenticationResetPasswordSuccess = 2004;
    public const int AuthenticationRefreshAccessTokenSuccess = 2005;

    public const int DataImportUserSucess = 2010;

    public const int ProfileChangeEmailTokenSuccess = 2020;
    public const int ProfileChangeEmailSuccess = 2021;
    public const int ProfileDeleteProfileImageSuccess = 2022;
    public const int ProfileVerifyEmailTokenSuccess = 2023;
    public const int ProfileSetProfileImageSuccess = 2024;
    public const int ProfileSetProfileLanguageSuccess = 2025;
    public const int ProfileSetProfileThemeSuccess = 2026;
    public const int ProfileSetUsernameSuccess = 2027;
    public const int ProfileVerifyEmailSuccess = 2028;
    public const int ProfileGetProfileImageSuccess = 2029;

    // 4000 - 4999

    public const int AuthenticationInvalidCredentials = 4000;
    public const int AuthenticationInvalidUserId = 4001;
    public const int AuthenticationInvalidUsername = 4002;
    public const int AuthenticationLockedAccount = 4003;
    public const int AuthenticationInternalError = 4004;
    public const int AuthenticationMissingRefreshToken = 4005;
    public const int AuthenticationInvalidRefreshToken = 4006;

    public const int DataImportMissingData = 4010;
    public const int DataImportInsertFailed = 4011;
    public const int DataImportMissingFile = 4012;
    public const int DataImportUnsupportedFormat = 4013;

    public const int ProfileUserNotFound = 4020;
    public const int ProfileUserUpdateFailed = 4021;
    public const int ProfileEmailIdentical = 4022;
    public const int ProfileEmailNotIdentical = 4023;
    public const int ProfileInvalidToken = 4024;
}
