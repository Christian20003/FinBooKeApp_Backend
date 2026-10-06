namespace FinBooKeApp.Core.Shared.Authentication.Models;

public class ClaimException : Exception
{
    public ClaimException()
        : base() { }

    public ClaimException(string? msg)
        : base(msg) { }

    public ClaimException(string? msg, Exception exception)
        : base(msg, exception) { }
}
