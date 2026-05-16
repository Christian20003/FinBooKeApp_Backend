using Microsoft.Extensions.Localization;

namespace FinBooKeAPI.Logic.Email;

public class EmailTemplateBuilder(IStringLocalizer<EmailTemplateBuilder> localizer)
    : IEmailTemplateBuilder
{
    private const string RESET_PWD_TEMPLATE_PATH = "Templates/Email/PasswordReset.html";
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

    public string GetResetPasswordTemplate(string link)
    {
        var body = File.ReadAllText(RESET_PWD_TEMPLATE_PATH);
        body = ModifyBody(body, ResetPasswordReplacements);
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
