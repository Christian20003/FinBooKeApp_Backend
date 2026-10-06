using System.Security.Claims;
using FinBooKeAPI.Logic.Authentication;
using FinBooKeAPI.Models.Exceptions;

namespace FinBooKeAPI.Tests.Logic.Authentication;

public class ClaimProviderUnitTest
{
    private const string EMAIL = "example@gmx.com";
    private const string USER_ID = "123456789";

    private readonly ClaimProvider _provider;

    public ClaimProviderUnitTest()
    {
        _provider = new ClaimProvider();
    }

    [Fact]
    public void Should_CreateAListWithAllClaims()
    {
        var claims = _provider.CreateClaims(USER_ID, EMAIL);

        Assert.Equal(2, claims.Count());
    }

    [Fact]
    public void Should_CreateEmailClaim()
    {
        var claims = _provider.CreateClaims(USER_ID, EMAIL);
        var emailClaim = claims.First(claim => claim.Type == ClaimTypes.Email);

        Assert.Equal(EMAIL, emailClaim.Value);
    }

    [Fact]
    public void Should_CreateUserIdClaim()
    {
        var claims = _provider.CreateClaims(USER_ID, EMAIL);
        var userIdClaim = claims.First(claim => claim.Type == ClaimTypes.NameIdentifier);

        Assert.Equal(USER_ID, userIdClaim.Value);
    }

    [Fact]
    public void Should_ReturnEmailFromClaims()
    {
        var claims = new List<Claim> { new(ClaimTypes.Email, EMAIL) };
        var principle = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var email = _provider.GetEmail(principle);

        Assert.Equal(EMAIL, email);
    }

    [Fact]
    public void Should_ReturnUserIdFromClaims()
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, USER_ID) };
        var principle = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var userId = _provider.GetUserId(principle);

        Assert.Equal(USER_ID, userId);
    }

    [Fact]
    public void Should_ReturnExpiresFromClaims()
    {
        var date = DateTimeOffset.UtcNow;
        var claims = new List<Claim> { new("exp", date.ToUnixTimeSeconds().ToString()) };
        var principle = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var expires = _provider.GetExpires(principle);

        Assert.Equal(date.Date, expires.Date);
        Assert.Equal(date.Hour, expires.Hour);
        Assert.Equal(date.Minute, expires.Minute);
    }

    [Fact]
    public void Should_ThrowException_WhenEmailClaimNotFound()
    {
        var claims = new List<Claim>();
        var principle = new ClaimsPrincipal(new ClaimsIdentity(claims));

        Assert.Throws<ClaimException>(() => _provider.GetEmail(principle));
    }

    [Fact]
    public void Should_ThrowException_WhenUserIdClaimNotFound()
    {
        var claims = new List<Claim>();
        var principle = new ClaimsPrincipal(new ClaimsIdentity(claims));

        Assert.Throws<ClaimException>(() => _provider.GetUserId(principle));
    }

    [Fact]
    public void Should_ThrowException_WhenExpiresClaimNotFound()
    {
        var claims = new List<Claim>();
        var principle = new ClaimsPrincipal(new ClaimsIdentity(claims));

        Assert.Throws<ClaimException>(() => _provider.GetExpires(principle));
    }
}
