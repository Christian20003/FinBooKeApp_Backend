namespace FinBooKeAPI.Logic.Parsing;

public interface IParserFactory
{
    public IParser GetParser(ParserType contentType);
}
