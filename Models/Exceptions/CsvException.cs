namespace FinBooKeAPI.Models.Exceptions;

public class CsvException : Exception
{
    public CsvException()
        : base() { }

    public CsvException(string? msg)
        : base(msg) { }

    public CsvException(string? msg, Exception exception)
        : base(msg, exception) { }
}
