using System.Net;
using System.Security.Authentication;
using FinBookeAPI.DTO.Error;
using FinBookeAPI.Models.Exceptions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FinBookeAPI.Middleware;

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
        var body = new ErrorDTO
        {
            Status = context.Response.StatusCode,
            Instance =
                context.Request.Scheme + "://" + context.Request.Host.Value + context.Request.Path,
        };
        switch (exception)
        {
            case InvalidCredentialException:
            {
                body.Type = "AuthenticationException";
                body.Title = "Invalid credentials";
                body.Detail = "Provided email or password is not valid";
                body.Status = (int)HttpStatusCode.Forbidden;
                break;
            }
            case FileNotFoundException:
            case EntityNotFoundException:
            {
                body.Type = "EntityNotFoundException";
                body.Title = "Requested entity not found";
                body.Detail = "The requested resource could not be found in the database";
                body.Status = (int)HttpStatusCode.NotFound;
                break;
            }
            case AuthorizationException:
            {
                body.Type = "AuthorizationException";
                body.Title = "Unauthorized access";
                body.Detail = exception.Message;
                body.Status = (int)HttpStatusCode.Unauthorized;
                break;
            }
            case SecurityTokenException:
            case SecurityTokenMalformedException:
            {
                body.Type = "AuthenticationException";
                body.Title = "Invalid token";
                body.Detail = "Provided authentication token is not valid";
                body.Status = (int)HttpStatusCode.Forbidden;
                break;
            }
            case FormatException:
            case ArgumentException:
            {
                body.Type = "ArgumentException";
                body.Title = "Invalid argument";
                body.Detail = exception.Message;
                body.Status = (int)HttpStatusCode.BadRequest;
                break;
            }
            default:
            {
                body.Type = "UnexpectedException";
                body.Title = "Unexpected failure";
                body.Detail = "Requested operation failed due to an unexpected server failure";
                body.Status = (int)HttpStatusCode.InternalServerError;
                break;
            }
        }
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = body.Status;
        return context.Response.WriteAsync(JsonConvert.SerializeObject(body));
    }
}
