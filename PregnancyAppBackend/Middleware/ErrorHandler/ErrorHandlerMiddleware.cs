using PregnancyAppBackend.Exceptions;
using System.Net;
using System.Text.Json;

namespace PregnancyAppBackend.Middleware.ErrorHandler;

public class ErrorHandlerMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlerMiddleware> logger,
    IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (ApiException ex)
        {
            await HandleApiExceptionAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleApiExceptionAsync(HttpContext httpContext, ApiException exception)
    {

        if (!string.IsNullOrWhiteSpace(exception.Message))
            logger.LogWarning(exception.Message);
        if (!string.IsNullOrWhiteSpace(exception.UserFriendlyMessage))
            logger.LogWarning(exception.UserFriendlyMessage);

        return WriteResponseAsync(httpContext,
                                  exception.HttpStatusCode,
                                  exception.Message,
                                  exception.UserFriendlyMessage,
                                  exception.StackTrace);
    }

    private Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {

        if (!string.IsNullOrWhiteSpace(exception.Message))
            logger.LogError(exception, exception.Message);

        return WriteResponseAsync(httpContext,
                                  HttpStatusCode.InternalServerError,
                                  exception.Message,
                                  "Внутренняя ошибка сервера.",
                                  exception.StackTrace);
    }

    private Task WriteResponseAsync(HttpContext context,
                                    HttpStatusCode httpStatusCode,
                                    string errorMessage,
                                    string userFriendlyMessage,
                                    string details)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;

        var errorResponse = env.IsDevelopment()
               ? new ErrorResponse(errorMessage, userFriendlyMessage, details)
               : new ErrorResponse(errorMessage, userFriendlyMessage);

        var serializerSettings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        string json = JsonSerializer.Serialize(errorResponse, serializerSettings);

        return context.Response.WriteAsync(json);
    }
}