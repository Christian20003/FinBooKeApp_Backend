using System.Diagnostics;
using FinBooKeAPI.Models.DTO.Error;
using FinBookeAPI.Models.Result;

namespace FinBooKeAPI.Mapping.Error;

public static class ErrorMapper
{
    public static BaseErrorDTO GetErrorDTO(
        List<string> errors,
        ErrorType errorType,
        string multipleErrorKey = ""
    )
    {
        var traceId = Activity.Current?.Id ?? "";
        return errorType switch
        {
            ErrorType.BAD_REQUEST => GetBadRequestDTO(errors, multipleErrorKey, traceId),
            ErrorType.FORBIDDEN => GetForbiddenDTO(errors, traceId),
            ErrorType.UNAUTHORIZED => GetUnauthorizedDTO(traceId),
            _ => GetInternalErrorDTO(errors, traceId),
        };
    }

    private static MultipleErrorDTO GetBadRequestDTO(
        List<string> errors,
        string errorKey,
        string traceId
    )
    {
        return new MultipleErrorDTO
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "One or more validation errors occurred.",
            Status = 400,
            Errors = new() { { errorKey, errors.ToArray() } },
            TraceId = traceId,
        };
    }

    private static SingleErrorDTO GetForbiddenDTO(List<string> errors, string traceId)
    {
        return new SingleErrorDTO
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            Title = "Access on this endpoint is forbidden.",
            Status = 403,
            Error = errors.FirstOrDefault() ?? "",
            TraceId = traceId,
        };
    }

    private static SingleErrorDTO GetUnauthorizedDTO(string traceId)
    {
        return new SingleErrorDTO
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            Title = "Access on this endpoint is forbidden.",
            Status = 401,
            Error = "",
            TraceId = traceId,
        };
    }

    private static SingleErrorDTO GetInternalErrorDTO(List<string> errors, string traceId)
    {
        return new SingleErrorDTO
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            Title = "An internal server error occurred.",
            Status = 500,
            Error = errors.FirstOrDefault() ?? "",
            TraceId = traceId,
        };
    }
}
