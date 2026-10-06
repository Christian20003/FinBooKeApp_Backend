namespace FinBooKeAPI.Logic.Parsing;

public class ParserFactory(IServiceProvider provider) : IParserFactory
{
    private readonly IServiceProvider _provider = provider;

    public IParser GetParser(ParserType contentType)
    {
        if (contentType == ParserType.JSON)
            return (IParser)_provider.GetRequiredService(typeof(JsonParser));
        if (contentType == ParserType.XML)
            return (IParser)_provider.GetRequiredService(typeof(XmlParser));
        if (contentType == ParserType.CSV)
            return (IParser)_provider.GetRequiredService(typeof(CsvParser));
        throw new NotSupportedException($"Format '{contentType}' is not supported.");
    }
}
