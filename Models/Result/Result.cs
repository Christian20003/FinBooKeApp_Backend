namespace FinBookeAPI.Models.Result;

public record Result<T>(bool HasValue, T? Value, ErrorType ErrorType, List<string> ErrorMessages);

public record Result<T, E>
    where E : Enum
{
    public T? Value { get; }
    public E ErrorCode { get; }

    public Result(T value)
    {
        Value = value;
        ErrorCode = default!;
    }

    public Result(E errorCode)
    {
        Value = default;
        ErrorCode = errorCode;
    }

    public bool HasValue()
    {
        return Value != null && EqualityComparer<E>.Default.Equals(ErrorCode, default);
    }
};

public static class Result
{
    public static Result<T> Ok<T>(T value) => new(true, value, ErrorType.NONE, [""]);

    public static Result<T> BadRequest<T>(List<string> messages) =>
        new(false, default, ErrorType.BAD_REQUEST, messages);

    public static Result<T> BadRequest<T>(string message) =>
        new(false, default, ErrorType.BAD_REQUEST, [message]);

    public static Result<T> Forbidden<T>(string message) =>
        new(false, default, ErrorType.FORBIDDEN, [message]);

    public static Result<T> Unauthorized<T>(string message) =>
        new(false, default, ErrorType.UNAUTHORIZED, [message]);

    public static Result<T> NotFound<T>(string message) =>
        new(false, default, ErrorType.NOT_FOUND, [message]);

    public static Result<T> InternalError<T>(List<string> messages) =>
        new(false, default, ErrorType.INTERNAL_ERROR, messages);

    public static Result<T> InternalError<T>(string message) =>
        new(false, default, ErrorType.INTERNAL_ERROR, [message]);

    public static Result<T, E> Ok<T, E>(T value)
        where E : Enum => new(value);

    public static Result<T, E> Error<T, E>(E error)
        where E : Enum => new(error);
}
