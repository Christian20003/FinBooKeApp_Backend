using FinBooKeApp.Core.Shared.Parsing.Models;

namespace FinBooKeApp.Core.Shared.Parsing.Interfaces;

public interface IParserFactory
{
    public IParser GetParser(ParserType contentType);
}
