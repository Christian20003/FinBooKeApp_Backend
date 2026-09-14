using FinBooKeAPI.Logic.Email;
using Moq;

namespace FinBooKeAPI.Tests.Mocks.Dependencies;

public static class MockEmailTemplateBuilder
{
    public static Mock<IEmailTemplateBuilder> GetMock()
    {
        var mock = new Mock<IEmailTemplateBuilder>();
        mock.Setup(obj => obj.GetResetPasswordTemplate(It.IsAny<string>()))
            .Returns(
                (string value) =>
                {
                    return $"template-{value}";
                }
            );
        mock.Setup(obj => obj.GetChangeEmailTemplate(It.IsAny<string>()))
            .Returns(
                (string value) =>
                {
                    return $"template-{value}";
                }
            );
        mock.Setup(obj => obj.GetVerifyEmailTemplate(It.IsAny<string>()))
            .Returns(
                (string value) =>
                {
                    return $"template-{value}";
                }
            );
        return mock;
    }
}
