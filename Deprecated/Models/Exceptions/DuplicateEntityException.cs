namespace FinBookeAPI.Models.Exceptions;

public class DuplicateEntityException : Exception
{
    public DuplicateEntityException()
        : base() { }

    public DuplicateEntityException(string? msg)
        : base(msg) { }

    public DuplicateEntityException(string? msg, Exception exception)
        : base(msg, exception) { }
}
