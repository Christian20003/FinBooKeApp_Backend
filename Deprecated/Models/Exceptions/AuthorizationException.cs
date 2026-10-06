namespace FinBookeAPI.Models.Exceptions;

public class AuthorizationException : Exception
{
    public AuthorizationException()
        : base() { }

    public AuthorizationException(string? msg)
        : base(msg) { }

    public AuthorizationException(string? msg, Exception exception)
        : base(msg, exception) { }
}
