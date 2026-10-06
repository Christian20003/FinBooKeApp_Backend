using System.Security.Claims;
using FinBooKeAPI.Logic.Authentication;
using FinBooKeAPI.Models.Logic.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace FinBooKeAPI.Tests.Logic.Authentication;

public partial class TokenProviderUnitTest
{
    private readonly TokenProvider _provider;
    private readonly CreateTokenPayload _createTokenPayload;
    private readonly VerifyTokenPayload _verifyTokenPayload;
    private readonly List<Claim> _claims;

    public TokenProviderUnitTest()
    {
        _provider = new TokenProvider();
        _claims =
        [
            new(ClaimTypes.NameIdentifier, "Identifier"),
            new(ClaimTypes.Email, "example@gmx.com"),
        ];
        _createTokenPayload = new CreateTokenPayload
        {
            Issuer = "ExampleIssuer",
            Audience = "ExampleAudience",
            Secret = "ExampleSecretWithMoreThan32Characters",
            Expires = DateTime.Now.AddMinutes(10.0),
            Claims = _claims,
        };
        _verifyTokenPayload = new VerifyTokenPayload
        {
            Issuer = _createTokenPayload.Issuer,
            Audience = _createTokenPayload.Audience,
            Secret = _createTokenPayload.Secret,
            Token = "",
        };
    }

    [Fact]
    public void Should_CreateNonEmptyToken()
    {
        var token = _provider.CreateToken(_createTokenPayload);
        Assert.NotEqual(0, token.Value.Length);
    }

    [Fact]
    public void Should_CreateTokenWithDefinedExpireDate()
    {
        var token = _provider.CreateToken(_createTokenPayload);
        Assert.Equal(_createTokenPayload.Expires.Ticks, token.Expires);
    }

    [Fact]
    public void Should_ReturnClaims_WhenTokenIsValid()
    {
        var token = _provider.CreateToken(_createTokenPayload);
        _verifyTokenPayload.Token = token.Value;
        var claim = _provider.VerifyToken(_verifyTokenPayload);
        var claims = claim.Claims;

        var expectedIdentifier = claims
            .First(claim => claim.Type == ClaimTypes.NameIdentifier)
            .Value;
        var expectedEmail = claims.First(claim => claim.Type == ClaimTypes.Email).Value;
        var actualIdentifier = _claims
            .First(claim => claim.Type == ClaimTypes.NameIdentifier)
            .Value;
        var actualEmail = _claims.First(claim => claim.Type == ClaimTypes.Email).Value;

        Assert.Equal(expectedIdentifier, actualIdentifier);
        Assert.Equal(expectedEmail, actualEmail);
    }

    [Fact]
    public void Should_ThrowException_WhenIssuerIsInvalid()
    {
        var token = _provider.CreateToken(_createTokenPayload);
        _verifyTokenPayload.Token = token.Value;
        _verifyTokenPayload.Issuer = "InvalidIssuer";

        Assert.ThrowsAny<SecurityTokenException>(() => _provider.VerifyToken(_verifyTokenPayload));
    }

    [Fact]
    public void Should_ThrowException_WhenAudienceIsInvalid()
    {
        var token = _provider.CreateToken(_createTokenPayload);
        _verifyTokenPayload.Token = token.Value;
        _verifyTokenPayload.Audience = "InvalidAudience";

        Assert.ThrowsAny<SecurityTokenException>(() => _provider.VerifyToken(_verifyTokenPayload));
    }

    [Fact]
    public void Should_ThrowException_WhenSecretIsInvalid()
    {
        var token = _provider.CreateToken(_createTokenPayload);
        _verifyTokenPayload.Token = token.Value;
        _verifyTokenPayload.Secret = "InvalidSecretWithMoreThan32Characters";

        Assert.ThrowsAny<SecurityTokenException>(() => _provider.VerifyToken(_verifyTokenPayload));
    }

    [Fact]
    public void Should_ThrowException_WhenTokenIsInvalid()
    {
        _verifyTokenPayload.Token = "InvalidToken";

        Assert.ThrowsAny<SecurityTokenMalformedException>(
            () => _provider.VerifyToken(_verifyTokenPayload)
        );
    }

    [Fact]
    public void Should_ThrowException_WhenTokenHasExpired()
    {
        _createTokenPayload.Expires = DateTime.Now.AddMinutes(-10.0);
        var token = _provider.CreateToken(_createTokenPayload);
        _verifyTokenPayload.Token = token.Value;

        Assert.ThrowsAny<SecurityTokenException>(() => _provider.VerifyToken(_verifyTokenPayload));
    }
}
