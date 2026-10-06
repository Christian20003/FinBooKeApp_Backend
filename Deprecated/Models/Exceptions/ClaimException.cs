namespace FinBooKeAPI.Models.Exceptions;

public class ClaimException : Exception
{
    public ClaimException()
        : base() { }

    public ClaimException(string? msg)
        : base(msg) { }

    public ClaimException(string? msg, Exception exception)
        : base(msg, exception) { }
}
