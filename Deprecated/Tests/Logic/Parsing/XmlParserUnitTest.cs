using System.Xml;
using FinBooKeAPI.Logic.Parsing;

namespace FinBooKeAPI.Tests.Logic.Parsing;

public class XmlParserUnitTest
{
    private readonly XmlParser _parser;

    public XmlParserUnitTest()
    {
        _parser = new XmlParser();
    }

    private static string GetSingleElement()
    {
        return @"<?xml version=""1.0"" encoding=""utf-8""?>
            <TestObjects>
              <TestObject>
                <GuidValue>c6d8eb11-8fd3-44f6-a7c9-cb5e03263188</GuidValue>
                <StringValue>This is a test string</StringValue>
                <LongValue>1234</LongValue>
                <DateTimeValue>2025-10-02</DateTimeValue>
                <ListValue>
                  <item>1</item>
                  <item>2</item>
                  <item>3</item>
                  <item>4</item>
                  <item>5</item>
                </ListValue>
              </TestObject>
            </TestObjects>";
    }

    private static string GetMultipleElements()
    {
        return @"<?xml version=""1.0"" encoding=""utf-8""?>
            <TestObjects>
              <TestObject>
                <GuidValue>c6d8eb11-8fd3-44f6-a7c9-cb5e03263188</GuidValue>
                <StringValue>This is a test string</StringValue>
                <LongValue>1234</LongValue>
                <DateTimeValue>2025-10-02</DateTimeValue>
                <ListValue>
                  <item>1</item>
                  <item>2</item>
                  <item>3</item>
                  <item>4</item>
                  <item>5</item>
                </ListValue>
              </TestObject>
              <TestObject>
                <GuidValue>c6d8eb11-8fd3-44f6-a7c9-cb5e03263188</GuidValue>
                <StringValue>This is a test string</StringValue>
                <LongValue>1234</LongValue>
                <DateTimeValue>2025-10-02</DateTimeValue>
                <ListValue>
                  <item>1</item>
                  <item>2</item>
                  <item>3</item>
                  <item>4</item>
                  <item>5</item>
                </ListValue>
              </TestObject>
            </TestObjects>";
    }

    [Fact]
    public void Parse_WhenContentIsNotEmpty_ReturnNonEmptyList()
    {
        var content = GetSingleElement();

        var result = _parser.Parse<TestObject>(content);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Parse_WhenContentIsInXmlFormat_ReturnParsedElement()
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
            @"<?xml version=""1.0"" encoding=""utf-8""?>
            <TestObjects>
              <TestObject>
                <GuidValue>c6d8eb11-8fd3-44f6-a7c9-cb5e03263188</GuidValue>
                <StringValue>This is a test string</StringValue>
                <LongValue>1234</LongValue>
              </TestObject>
            </TestObjects>";

        var result = _parser.Parse<TestObject>(content);
        var element = result.First();

        Assert.Equal(DateTime.MinValue, element.DateTimeValue);
        Assert.Empty(element.ListValue);
    }

    [Fact]
    public void Parse_WhenContentIsEmpty_ThrowException()
    {
        Assert.Throws<XmlException>(() => _parser.Parse<TestObject>(""));
    }

    [Fact]
    public void Parse_WhenContentIsNotValidXml_ThrowException()
    {
        Assert.Throws<XmlException>(() => _parser.Parse<TestObject>("This is invalid xml content"));
    }
}
