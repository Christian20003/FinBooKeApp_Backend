using FinBooKeAPI.Logic.Parsing;
using FinBooKeAPI.Models.Exceptions;

namespace FinBooKeAPI.Tests.Logic.Parsing;

public class CsvParserUnitTest
{
    private readonly CsvParser _parser;

    public CsvParserUnitTest()
    {
        _parser = new CsvParser();
    }

    private static string GetSingleElement()
    {
        return @"GuidValue,StringValue,LongValue,DateTimeValue,ListValue
        ""c6d8eb11-8fd3-44f6-a7c9-cb5e03263188"",""This is a test string"",1234,""2025-10-02"",""1;2;3;4;5""";
    }

    private static string GetMultipleElements()
    {
        return @"GuidValue,StringValue,LongValue,DateTimeValue,ListValue
        ""c6d8eb11-8fd3-44f6-a7c9-cb5e03263188"",""This is a test string"",1234,""2025-10-02"",""1;2;3;4;5""
        ""c6d8eb11-8fd3-44f6-a7c9-cb5e03263188"",""This is a test string"",1234,""2025-10-02"",""1;2;3;4;5""";
    }

    [Fact]
    public void Parse_WhenContentIsNotEmpty_ReturnNonEmptyList()
    {
        var content = GetSingleElement();

        var result = _parser.Parse<TestObject>(content);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Parse_WhenContentIsInCsvFormat_ReturnParsedElement()
    {
        var content = GetSingleElement();

        var result = _parser.Parse<TestObject>(content);

        foreach (var obj in result)
        {
            Assert.NotEqual(Guid.Empty, obj.GuidValue);
            Assert.NotEqual(string.Empty, obj.StringValue);
            Assert.NotEqual(0, obj.LongValue);
            Assert.NotEqual(DateTime.MinValue, obj.DateTimeValue);
            Assert.NotEmpty(obj.ListValue);
        }
    }

    [Fact]
    public void Parse_WhenContentContainsMultipleElements_ReturnAListOfParsedElements()
    {
        var content = GetMultipleElements();

        var result = _parser.Parse<TestObject>(content);

        Assert.NotEqual(1, result.Count());
    }

    [Fact]
    public void Parse_WhenPropertyIsNotDefined_UseDefaultValue()
    {
        var content =
            @"GuidValue,StringValue,LongValue
        ""c6d8eb11-8fd3-44f6-a7c9-cb5e03263188"",""This is a test string"",1234";

        var result = _parser.Parse<TestObject>(content);
        var element = result.First();

        Assert.Equal(DateTime.MinValue, element.DateTimeValue);
        Assert.Empty(element.ListValue);
    }

    [Fact]
    public void Parse_WhenContentIsEmpty_ThrowException()
    {
        Assert.Throws<CsvException>(() => _parser.Parse<TestObject>(""));
    }

    [Fact]
    public void Parse_WhenContentIsNotValidCsv_ThrowException()
    {
        var content =
            @"GuidValue,StringValue,LongValue,AnotherField
        ""c6d8eb11-8fd3-44f6-a7c9-cb5e03263188"",""This is a test string"",1234";

        Assert.Throws<CsvException>(() => _parser.Parse<TestObject>(content));
    }
}
