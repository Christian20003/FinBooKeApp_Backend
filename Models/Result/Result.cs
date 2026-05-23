namespace FinBookeAPI.Models.Result;

public record Result<T>(bool HasValue, T? Value, ErrorType ErrorType, string ErrorMessage);

public static class Result
{
    public static Result<T> Ok<T>(T value) => new(true, value, ErrorType.NONE, "");

    public static Result<T> BadRequest<T>(string message) =>
        new(false, default, ErrorType.BAD_REQUEST, message);

    public static Result<T> Forbidden<T>(string message) =>
        new(false, default, ErrorType.FORBIDDEN, message);
}
