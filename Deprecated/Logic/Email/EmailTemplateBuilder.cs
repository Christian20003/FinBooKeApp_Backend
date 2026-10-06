using Microsoft.Extensions.Localization;

namespace FinBooKeAPI.Logic.Email;

public class EmailTemplateBuilder(IStringLocalizer<EmailTemplateBuilder> localizer)
    : IEmailTemplateBuilder
{
    private const string GET_TOKEN_TEMPLATE_PATH = "Templates/Email/GetTokenTemplate.html";
    private const string SUPPORT_MAIL = "support@example.com";

    private readonly List<(string key, string value)> ResetPasswordReplacements =
    [
        ("{{title}}", localizer.GetString("PasswordResetTitle")),
        ("{{header}}", localizer.GetString("PasswordResetHeader")),
        ("{{greeting}}", localizer.GetString("Greeting")),
        ("{{introduction}}", localizer.GetString("PasswordResetIntroduction")),
        ("{{linkButton}}", localizer.GetString("PasswordResetButton")),
        ("{{warning}}", localizer.GetString("PasswordResetWarning")),
        ("{{alternative}}", localizer.GetString("PasswordResetAlternative")),
        ("{{valediction}}", localizer.GetString("Valediction")),
        ("{{year}}", DateTime.Now.Year.ToString()),
        ("{{qa}}", localizer.GetString("QA")),
        ("{{qaMail}}", SUPPORT_MAIL),
    ];

    private readonly List<(string key, string value)> ChangeEmailReplacements =
    [
        ("{{title}}", localizer.GetString("ChangeEmailTitle")),
        ("{{header}}", localizer.GetString("ChangeEmailHeader")),
        ("{{greeting}}", localizer.GetString("Greeting")),
        ("{{introduction}}", localizer.GetString("ChangeEmailIntroduction")),
        ("{{linkButton}}", localizer.GetString("ChangeEmailButton")),
        ("{{warning}}", localizer.GetString("ChangeEmailWarning")),
        ("{{alternative}}", localizer.GetString("ChangeEmailAlternative")),
        ("{{valediction}}", localizer.GetString("Valediction")),
        ("{{year}}", DateTime.Now.Year.ToString()),
        ("{{qa}}", localizer.GetString("QA")),
        ("{{qaMail}}", SUPPORT_MAIL),
    ];

    private readonly List<(string key, string value)> VerifyEmailReplacements =
    [
        ("{{title}}", localizer.GetString("VerifyEmailTitle")),
        ("{{header}}", localizer.GetString("VerifyEmailHeader")),
        ("{{greeting}}", localizer.GetString("Greeting")),
        ("{{introduction}}", localizer.GetString("VerifyEmailIntroduction")),
        ("{{linkButton}}", localizer.GetString("VerifyEmailButton")),
        ("{{warning}}", localizer.GetString("VerifyEmailWarning")),
        ("{{alternative}}", localizer.GetString("VerifyEmailAlternative")),
        ("{{valediction}}", localizer.GetString("Valediction")),
        ("{{year}}", DateTime.Now.Year.ToString()),
        ("{{qa}}", localizer.GetString("QA")),
        ("{{qaMail}}", SUPPORT_MAIL),
    ];

    public string GetResetPasswordTemplate(string link)
    {
        var body = File.ReadAllText(GET_TOKEN_TEMPLATE_PATH);
        body = ModifyBody(body, ResetPasswordReplacements);
        body = body.Replace("{{link}}", link);
        return body;
    }

    public string GetChangeEmailTemplate(string link)
    {
        var body = File.ReadAllText(GET_TOKEN_TEMPLATE_PATH);
        body = ModifyBody(body, ChangeEmailReplacements);
        body = body.Replace("{{link}}", link);
        return body;
    }

    public string GetVerifyEmailTemplate(string link)
    {
        var body = File.ReadAllText(GET_TOKEN_TEMPLATE_PATH);
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
