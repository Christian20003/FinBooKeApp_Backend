using FinBooKeApp.Core.Shared.Email.Interfaces;
using FinBooKeApp.Core.Shared.FileSystem.Interfaces;

namespace FinBooKeApp.Core.Shared.Email.Providers;

public class EmailTemplateBuilder(IFileSystem fileSystem) : IEmailTemplateBuilder
{
    private readonly IFileSystem _fileSystem = fileSystem;

    private const string GET_TOKEN_TEMPLATE_PATH = "Templates/Email/GetTokenTemplate.html";
    private const string SUPPORT_MAIL = "support@example.com";

    private readonly List<(string key, string value)> ResetPasswordReplacements =
    [
        ("{{title}}", EmailText.PasswordResetTitle),
        ("{{header}}", EmailText.PasswordResetHeader),
        ("{{greeting}}", EmailText.Greeting),
        ("{{introduction}}", EmailText.PasswordResetIntroduction),
        ("{{linkButton}}", EmailText.PasswordResetButton),
        ("{{warning}}", EmailText.PasswordResetWarning),
        ("{{alternative}}", EmailText.PasswordResetAlternative),
        ("{{valediction}}", EmailText.Valediction),
        ("{{year}}", DateTime.Now.Year.ToString()),
        ("{{qa}}", EmailText.QA),
        ("{{qaMail}}", SUPPORT_MAIL),
    ];

    private readonly List<(string key, string value)> ChangeEmailReplacements =
    [
        ("{{title}}", EmailText.ChangeEmailTitle),
        ("{{header}}", EmailText.ChangeEmailHeader),
        ("{{greeting}}", EmailText.Greeting),
        ("{{introduction}}", EmailText.ChangeEmailIntroduction),
        ("{{linkButton}}", EmailText.ChangeEmailButton),
        ("{{warning}}", EmailText.ChangeEmailWarning),
        ("{{alternative}}", EmailText.PasswordResetAlternative),
        ("{{valediction}}", EmailText.Valediction),
        ("{{year}}", DateTime.Now.Year.ToString()),
        ("{{qa}}", EmailText.QA),
        ("{{qaMail}}", SUPPORT_MAIL),
    ];

    private readonly List<(string key, string value)> VerifyEmailReplacements =
    [
        ("{{title}}", EmailText.VerifyEmailTitle),
        ("{{header}}", EmailText.VerifyEmailHeader),
        ("{{greeting}}", EmailText.Greeting),
        ("{{introduction}}", EmailText.VerifyEmailIntroduction),
        ("{{linkButton}}", EmailText.VerifyEmailButton),
        ("{{warning}}", EmailText.VerifyEmailWarning),
        ("{{alternative}}", EmailText.PasswordResetAlternative),
        ("{{valediction}}", EmailText.Valediction),
        ("{{year}}", DateTime.Now.Year.ToString()),
        ("{{qa}}", EmailText.QA),
        ("{{qaMail}}", SUPPORT_MAIL),
    ];

    public string GetResetPasswordTemplate(string link)
    {
        var body = _fileSystem.ReadAllText(GET_TOKEN_TEMPLATE_PATH);
        body = ModifyBody(body, ResetPasswordReplacements);
        body = body.Replace("{{link}}", link);
        return body;
    }

    public string GetChangeEmailTemplate(string link)
    {
        var body = _fileSystem.ReadAllText(GET_TOKEN_TEMPLATE_PATH);
        body = ModifyBody(body, ChangeEmailReplacements);
        body = body.Replace("{{link}}", link);
        return body;
    }

    public string GetVerifyEmailTemplate(string link)
    {
        var body = _fileSystem.ReadAllText(GET_TOKEN_TEMPLATE_PATH);
        body = ModifyBody(body, VerifyEmailReplacements);
        body = body.Replace("{{link}}", link);
        return body;
    }

    private static string ModifyBody(string template, List<(string key, string value)> replacements)
    {
        foreach (var (key, value) in replacements)
        {
            template = template.Replace(key, value);
        }
        return template;
    }
}
