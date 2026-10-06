namespace FinBooKeAPI.Logic.Parsing;

public interface IParser
{
    public IEnumerable<TYPE> Parse<TYPE>(string content)
        where TYPE : new();
}
