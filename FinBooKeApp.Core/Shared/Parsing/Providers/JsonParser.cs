using System.Text.Json;
using FinBooKeApp.Core.Shared.Parsing.Interfaces;

namespace FinBooKeApp.Core.Shared.Parsing.Providers;

public class JsonParser : IParser
{
    public IEnumerable<TYPE> Parse<TYPE>(string content)
        where TYPE : new()
    {
        return JsonSerializer.Deserialize<TYPE[]>(content) ?? [];
    }
}
