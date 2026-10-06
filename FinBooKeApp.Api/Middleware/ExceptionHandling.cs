using System.Net;
using FinBooKeApp.Api.Shared.Error;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FinBooKeApp.Api.Middleware;

/// <summary>
/// This middleware processes all kinds of exception that can
/// occur in this application. Thereby it creates corresponding
/// error message which are sent to the client.
/// </summary>
public class ExceptionHandling(ILogger<ExceptionHandling> logger) : IMiddleware
{
    private readonly ILogger<ExceptionHandling> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Exception has been processed");
            await HandleException(context, exception);
        }
    }

    private Task HandleException(HttpContext context, Exception exception)
    {
        var body = new SingleErrorDTO
        {
            Type = "",
            Title = "",
            Error = "",
            Status = context.Response.StatusCode,
            TraceId = context.TraceIdentifier,
        };
        switch (exception)
        {
            case FileNotFoundException:
            {
                body.Type = "EntityNotFoundException";
                body.Title = "Requested entity not found";
                body.Error = "The requested resource could not be found in the database";
                body.Status = (int)HttpStatusCode.NotFound;
                break;
            }
            case SecurityTokenException:
            case SecurityTokenMalformedException:
            {
                body.Type = "AuthenticationException";
                body.Title = "Invalid token";
                body.Error = "Provided authentication token is not valid";
                body.Status = (int)HttpStatusCode.Forbidden;
                break;
            }
            case FormatException:
            case ArgumentException:
            {
                body.Type = "ArgumentException";
                body.Title = "Invalid argument";
                body.Error = exception.Message;
                body.Status = (int)HttpStatusCode.BadRequest;
                break;
            }
            default:
            {
                body.Type = "UnexpectedException";
                body.Title = "Unexpected failure";
                body.Error = "Requested operation failed due to an unexpected server failure";
                body.Status = (int)HttpStatusCode.InternalServerError;
                break;
            }
        }
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = body.Status;
        return context.Response.WriteAsync(JsonConvert.SerializeObject(body));
    }
}
