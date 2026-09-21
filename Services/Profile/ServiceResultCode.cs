namespace FinBooKeAPI.Services.Profile;

public enum ServiceResultCode
{
    OK = 0,
    USER_NOT_FOUND = 1,
    USER_UPDATE_FAILED = 2,
    EMAIL_IDENTICAL = 3,
    EMAIL_INVALID = 4,
    TOKEN_INVALID = 5,
}
