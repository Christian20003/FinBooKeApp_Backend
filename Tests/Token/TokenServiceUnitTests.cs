using FinBookeAPI.Collections.TokenCollection;
using FinBooKeAPI.Models.Settings;
using FinBookeAPI.Models.Token;
using FinBookeAPI.Services.Token;
using FinBookeAPI.Tests.Mocks.Collections;
using FinBookeAPI.Tests.Records;
using Microsoft.Extensions.Options;
using Moq;

namespace FinBookeAPI.Tests.Token;

public partial class TokenServiceUnitTests
{
    private readonly TokenService _service;
    private readonly Mock<ITokenCollection> _collection;
    private readonly Mock<IOptions<AuthenticationSettings>> _settings;
    private readonly AuthenticationSettings _options;
    private readonly JwtToken _token;
    private const string _userId = "6ccf3b8a-0886-4213-a998-4a0356cb68dd";

    public TokenServiceUnitTests()
    {
        var logger = new Mock<ILogger<TokenService>>();
        _settings = new Mock<IOptions<AuthenticationSettings>>();
        _collection = MockTokenCollection.GetMock();
        _options = JwtSettingsRecord.GetObject();
        _token = JwtTokenRecord.GetObject();

        _settings.Setup(obj => obj.Value).Returns(_options);

        _service = new TokenService(_collection.Object, _settings.Object, logger.Object);
    }
}
