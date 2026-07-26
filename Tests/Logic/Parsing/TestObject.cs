namespace FinBooKeAPI.Tests.Logic.Parsing;

public class TestObject
{
    public Guid GuidValue { get; set; } = Guid.Empty;
    public string StringValue { get; set; } = "";
    public long LongValue { get; set; } = 0;
    public DateTime DateTimeValue { get; set; } = DateTime.MinValue;
    public List<int> ListValue { get; set; } = [];
}
