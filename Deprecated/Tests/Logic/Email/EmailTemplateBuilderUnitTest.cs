using FinBooKeAPI.Logic.Email;
using Microsoft.Extensions.Localization;
using Moq;

namespace FinBooKeAPI.Tests.Logic.Email;

public class EmailTemplateBuilderUnitTest
{
    private readonly EmailTemplateBuilder _builder;
    private readonly Mock<IStringLocalizer<EmailTemplateBuilder>> _localizer;

    public EmailTemplateBuilderUnitTest()
    {
        _localizer = new Mock<IStringLocalizer<EmailTemplateBuilder>>();
        _localizer
            .Setup(obj => obj[It.IsAny<string>()])
            .Returns((string key) => new LocalizedString(key, string.Empty));

        _builder = new EmailTemplateBuilder(_localizer.Object);
    }

    [Fact]
    public void GetResetPasswordTemplate_RemoveAllTemplateParameters_InResetPasswordTemplate()
    {
        var result = _builder.GetResetPasswordTemplate("link");

        var hasBrackets = result.Contains("{{") || result.Contains("}}");
        Assert.False(hasBrackets);
    }

    [Fact]
    public void GetResetPasswordTemplate_AddLinkToResetPasswordTemplate()
    {
        var link = "http://example.com";
        var result = _builder.GetResetPasswordTemplate(link);

        var hasLink = result.Contains(link);
        Assert.True(hasLink);
    }

    [Fact]
    public void GetChangeEmailTemplate_RemoveAllTemplateParameters_InResetPasswordTemplate()
    {
        var result = _builder.GetChangeEmailTemplate("link");

        var hasBrackets = result.Contains("{{") || result.Contains("}}");
        Assert.False(hasBrackets);
    }

    [Fact]
    public void GetChangeEmailTemplate_AddLinkToResetPasswordTemplate()
    {
        var link = "http://example.com";
        var result = _builder.GetChangeEmailTemplate(link);

        var hasLink = result.Contains(link);
        Assert.True(hasLink);
    }

    [Fact]
    public void GetVerifyEmailTemplate_RemoveAllTemplateParameters_InResetPasswordTemplate()
    {
        var result = _builder.GetVerifyEmailTemplate("link");

        var hasBrackets = result.Contains("{{") || result.Contains("}}");
        Assert.False(hasBrackets);
    }

    [Fact]
    public void GetVerifyEmailTemplate_AddLinkToResetPasswordTemplate()
    {
        var link = "http://example.com";
        var result = _builder.GetVerifyEmailTemplate(link);

        var hasLink = result.Contains(link);
        Assert.True(hasLink);
    }
}
