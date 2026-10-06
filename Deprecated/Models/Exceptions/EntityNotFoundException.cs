namespace FinBookeAPI.Models.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException()
        : base() { }

    public EntityNotFoundException(string? msg)
        : base(msg) { }

    public EntityNotFoundException(string? msg, Exception exception)
        : base(msg, exception) { }
}
