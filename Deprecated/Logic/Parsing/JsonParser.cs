using System.Text.Json;

namespace FinBooKeAPI.Logic.Parsing;

public class JsonParser : IParser
{
    public IEnumerable<TYPE> Parse<TYPE>(string content)
        where TYPE : new()
    {
        return JsonSerializer.Deserialize<TYPE[]>(content) ?? [];
    }
}
