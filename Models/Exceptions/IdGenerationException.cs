namespace FinBookeAPI.Models.Exceptions;

public class IdGenerationException : Exception
{
    public IdGenerationException()
        : base() { }

    public IdGenerationException(string? msg)
        : base(msg) { }

    public IdGenerationException(string? msg, Exception exception)
        : base(msg, exception) { }
}
