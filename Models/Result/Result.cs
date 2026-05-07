namespace FinBookeAPI.Models.Result;

public record Result<T>(bool HasValue, T Value, ErrorType ErrorType, string ErrorMessage);

public static class Result
{
    public static Result<T> Ok<T>(T value) => new(true, value, ErrorType.NONE, "");
}
