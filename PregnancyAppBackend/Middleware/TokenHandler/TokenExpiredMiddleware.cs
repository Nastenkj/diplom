using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Infrastructure.Security;
using PregnancyAppBackend.Middleware.ErrorHandler;
using PregnancyAppBackend.Persistance;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace PregnancyAppBackend.Middleware.TokenHandler;

public class TokenExpiredMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, DatabaseContext databaseContext)
    {
        var userIdFromClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var dateTimeStringFromClaim = context.User?.FindFirst(Claims.DateOfChangeOfAccessRights)?.Value ?? string.Empty;

        if (userIdFromClaim != null)
        {
            var userId = Guid.Parse(userIdFromClaim);
            var user = await databaseContext.Users.AsNoTracking().SingleAsync(u => u.Id == userId);

            var dbTokenDateTimeStr = user.GetDateOfChangeOfAccessRightsTokenValue();

            if (dateTimeStringFromClaim != dbTokenDateTimeStr)
            {
                var errorMessage = "Token has expired.";
                var userFriendlyMessage = "Токен истёк.";

                await WriteResponseAsync(context,
                                         HttpStatusCode.Unauthorized,
                                         errorMessage,
                                         userFriendlyMessage);
            }
            else
            {
                await next(context);
            }
        }
        else
        {
            await next(context);
        }
    }

    private Task WriteResponseAsync(HttpContext context,
                                    HttpStatusCode httpStatusCode,
                                    string errorMessage,
                                    string userFriendlyMessage)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;

        var errorResponse = new ErrorResponse(errorMessage, userFriendlyMessage);

        var serializerSettings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(errorResponse, serializerSettings);

        return context.Response.WriteAsync(json);
    }
}