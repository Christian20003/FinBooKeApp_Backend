using FinBooKeAPI.Logic.Parsing;
using Moq;

namespace FinBooKeAPI.Tests.Logic.Parsing;

public class ParserFactoryUnitTest
{
    private readonly Mock<IServiceProvider> _serviceProvider;
    private readonly ParserFactory _factory;

    public ParserFactoryUnitTest()
    {
        _serviceProvider = new Mock<IServiceProvider>();
        _factory = new ParserFactory(_serviceProvider.Object);

        _serviceProvider
            .Setup(obj => obj.GetService(It.IsAny<Type>()))
            .Returns(
                (Type type) =>
                {
                    if (type == typeof(JsonParser))
                        return new JsonParser();
                    if (type == typeof(XmlParser))
                        return new XmlParser();
                    if (type == typeof(CsvParser))
                        return new CsvParser();
                    return new object();
                }
            );
    }

    [Fact]
    public void GetParser_WhenParserTypeIsJson_ReturnJsonParser()
    {
        var parser = _factory.GetParser(ParserType.JSON);

        Assert.IsType<JsonParser>(parser);
    }

    [Fact]
    public void GetParser_WhenParserTypeIsXml_ReturnXmlParser()
    {
        var parser = _factory.GetParser(ParserType.XML);

        Assert.IsType<XmlParser>(parser);
    }

    [Fact]
    public void GetParser_WhenParserTypeIsCsv_ReturnCsvParser()
    {
        var parser = _factory.GetParser(ParserType.CSV);

        Assert.IsType<CsvParser>(parser);
    }

    [Fact]
    public void GetParser_WhenParserTypeIsUnsupported_ThrowException()
    {
        Assert.Throws<NotSupportedException>(() => _factory.GetParser((ParserType)4));
    }
}
