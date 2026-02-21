namespace PregnancyAppBackend.Middleware.ErrorHandler;

public static class ErrorHandlerMiddlewareExtension
{
    public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder) => builder.UseMiddleware<ErrorHandlerMiddleware>();
}