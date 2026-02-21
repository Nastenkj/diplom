using System.Net;

namespace PregnancyAppBackend.Exceptions;

public class ApiException : Exception
{
    public HttpStatusCode HttpStatusCode { get; }
    public string? UserFriendlyMessage { get; set; }

    public ApiException(HttpStatusCode httpStatusCode, 
                        string? message = null,
                        string? userFriendlyMessage = null) : base(message)
    {
        HttpStatusCode = httpStatusCode;
        UserFriendlyMessage = userFriendlyMessage;
    }

    public ApiException(string? message = null, string? userFriendlyMessage = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : this(statusCode, message, userFriendlyMessage)
    {
    }
    
    public ApiException(string? message = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : this(statusCode, message, null)
    {
    }

    public override string ToString()
    {
        return $"{nameof(HttpStatusCode)}={HttpStatusCode}, {nameof(Message)}={Message}, {nameof(UserFriendlyMessage)}={UserFriendlyMessage}";
    }
}