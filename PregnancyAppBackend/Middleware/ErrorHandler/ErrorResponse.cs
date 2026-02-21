namespace PregnancyAppBackend.Middleware.ErrorHandler;

public class ErrorResponse(string errorMessage, string userFriendlyMessage, string details = "")
{
    public string ErrorMessage { get; } = errorMessage;
    public string UserFriendlyMessage { get; } = userFriendlyMessage;
    public string Details { get; } = details;
}