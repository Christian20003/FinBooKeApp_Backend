using FinBooKeApp.Api.Shared.Error;
using FinBooKeApp.Core.Shared.Authentication.Models;
using FinBooKeApp.Core.Shared.FileSystem.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FinBooKeApp.Api.Middleware;

public class ExceptionHandling : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleException(context, exception);
        }
    }

    private static Task HandleException(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case FileNotFoundException:
            {
                var body = new SingleErrorDTO
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                    Title = "Requested entity not found",
                    Error = ExceptionResponse.FileNotFound,
                    Status = StatusCodes.Status404NotFound,
                    TraceId = context.TraceIdentifier,
                };
                return WriteResponse(context, body);
            }
            case FileFormatException:
            {
                var body = new MultipleErrorDTO
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "Invalid file format",
                    Errors = new() { { "filename", [ExceptionResponse.InvalidFileFormat] } },
                    Status = StatusCodes.Status400BadRequest,
                    TraceId = context.TraceIdentifier,
                };
                return WriteResponse(context, body);
            }
            case FileSizeException:
            {
                var body = new MultipleErrorDTO
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "Invalid file size",
                    Errors = new() { { "filename", [ExceptionResponse.InvalidFileSize] } },
                    Status = StatusCodes.Status400BadRequest,
                    TraceId = context.TraceIdentifier,
                };
                return WriteResponse(context, body);
            }
            case SecurityTokenException:
            case SecurityTokenMalformedException:
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return context.Response.WriteAsync("");
            }
            case ClaimException:
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return context.Response.WriteAsync("");
            }
            default:
            {
                Console.WriteLine(exception);
                var body = new SingleErrorDTO
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                    Title = "Internal server error",
                    Error = ExceptionResponse.DefaultError,
                    Status = StatusCodes.Status500InternalServerError,
                    TraceId = context.TraceIdentifier,
                };
                return WriteResponse(context, body);
            }
        }
    }

    private static Task WriteResponse(HttpContext context, SingleErrorDTO errorDTO)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorDTO.Status;
        return context.Response.WriteAsync(JsonConvert.SerializeObject(errorDTO));
    }

    private static Task WriteResponse(HttpContext context, MultipleErrorDTO errorDTO)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorDTO.Status;
        return context.Response.WriteAsync(JsonConvert.SerializeObject(errorDTO));
    }
}
