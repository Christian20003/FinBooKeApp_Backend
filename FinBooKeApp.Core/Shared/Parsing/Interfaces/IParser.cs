namespace FinBooKeApp.Core.Shared.Parsing.Interfaces;

public interface IParser
{
    public IEnumerable<TYPE> Parse<TYPE>(string content)
        where TYPE : new();
}
