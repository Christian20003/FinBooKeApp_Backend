using System.Diagnostics;
using FinBooKeAPI.Models.DTO.Error;
using FinBookeAPI.Models.Result;

namespace FinBooKeAPI.Mapping.Error;

public static class ErrorMapper
{
    public static FailedRequestDTO GetFailedRequestDTO(List<string> errors, ErrorType errorType)
    {
        var traceId = Activity.Current?.Id ?? "";
        return errorType switch
        {
            ErrorType.FORBIDDEN => GetForbiddenDTO(errors, traceId),
            _ => GetInternalErrorDTO(errors, traceId),
        };
    }

    public static BadRequestDTO GetBadRequestDTO(List<string> errors, string errorKey)
    {
        var traceId = Activity.Current?.Id ?? "";
        return new BadRequestDTO
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "One or more validation errors occurred.",
            Status = 400,
            Errors = new() { { errorKey, errors.ToArray() } },
            TraceId = traceId,
        };
    }

    public static FailedRequestDTO GetForbiddenDTO(List<string> errors, string traceId)
    {
        return new FailedRequestDTO
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            Title = "Access on this endpoint is forbidden.",
            Status = 403,
            Error = errors.FirstOrDefault() ?? "",
            TraceId = traceId,
        };
    }

    public static FailedRequestDTO GetUnauthorizedDTO(string traceId)
    {
        return new FailedRequestDTO
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            Title = "Access on this endpoint is forbidden.",
            Status = 401,
            Error = "",
            TraceId = traceId,
        };
    }

    public static FailedRequestDTO GetInternalErrorDTO(List<string> errors, string traceId)
    {
        return new FailedRequestDTO
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            Title = "An internal server error occurred.",
            Status = 500,
            Error = errors.FirstOrDefault() ?? "",
            TraceId = traceId,
        };
    }
}
