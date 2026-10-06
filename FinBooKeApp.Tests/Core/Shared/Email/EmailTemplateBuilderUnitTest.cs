using FinBooKeApp.Core.Shared.Email.Providers;
using FinBooKeApp.Core.Shared.FileSystem.Interfaces;
using Moq;

namespace FinBooKeApp.Tests.Core.Shared.Email;

public class EmailTemplateBuilderUnitTest
{
    private readonly EmailTemplateBuilder _builder;

    public EmailTemplateBuilderUnitTest()
    {
        var fileSystem = new Mock<IFileSystem>();
        fileSystem.Setup(obj => obj.ReadAllText(It.IsAny<string>())).Returns("{{link}}");

        _builder = new EmailTemplateBuilder(fileSystem.Object);
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
