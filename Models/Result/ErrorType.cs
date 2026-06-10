namespace FinBookeAPI.Models.Result;

public enum ErrorType
{
    NONE = 0,
    BAD_REQUEST = 1,
    FORBIDDEN = 2,
    UNAUTHORIZED = 3,
    INTERNAL_ERROR = 4,
}
