using System.Text.Json;
using FinBooKeAPI.Logic.Parsing;

namespace FinBooKeAPI.Tests.Logic.Parsing;

public class JsonParserUnitTest
{
    private readonly JsonParser _parser;

    public JsonParserUnitTest()
    {
        _parser = new JsonParser();
    }

    private static string GetSingleElement()
    {
        return @"[{
            ""GuidValue"": ""c6d8eb11-8fd3-44f6-a7c9-cb5e03263188"",
            ""StringValue"": ""This is a test string"",
            ""LongValue"": 1234,
            ""DateTimeValue"": ""2025-10-02"",
            ""ListValue"": [1,2,3,4,5]
        }]";
    }

    private static string GetMultipleElements()
    {
        return @"[
        {
            ""GuidValue"": ""c6d8eb11-8fd3-44f6-a7c9-cb5e03263188"",
            ""StringValue"": ""This is a test string"",
            ""LongValue"": 1234,
            ""DateTimeValue"": ""2025-10-02"",
            ""ListValue"": [1,2,3,4,5]
        },
        {
            ""GuidValue"": ""c6d8eb11-8fd3-44f6-a7c9-cb5e03263188"",
            ""StringValue"": ""This is a test string"",
            ""LongValue"": 1234,
            ""DateTimeValue"": ""2025-10-02"",
            ""ListValue"": [1,2,3,4,5]
        }
        ]";
    }

    [Fact]
    public void Parse_WhenContentIsNotEmpty_ReturnNonEmptyList()
    {
        var content = GetSingleElement();

        var result = _parser.Parse<TestObject>(content);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Parse_WhenContentIsInJsonFormat_ReturnParsedElement()
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
            @"[{
            ""GuidValue"": ""c6d8eb11-8fd3-44f6-a7c9-cb5e03263188"",
            ""StringValue"": ""This is a test string"",
            ""LongValue"": 1234
        }]";

        var result = _parser.Parse<TestObject>(content);
        var element = result.First();

        Assert.Equal(DateTime.MinValue, element.DateTimeValue);
        Assert.Empty(element.ListValue);
    }

    [Fact]
    public void Parse_WhenContentIsNull_ReturnEmptyList()
    {
        var result = _parser.Parse<TestObject>("null");

        Assert.Empty(result);
    }

    [Fact]
    public void Parse_WhenContentIsEmpty_ThrowException()
    {
        Assert.Throws<JsonException>(() => _parser.Parse<TestObject>(""));
    }

    [Fact]
    public void Parse_WhenContentIsNotValidJson_ThrowException()
    {
        Assert.Throws<JsonException>(
            () => _parser.Parse<TestObject>("This is invalid json content")
        );
    }
}
