namespace FinBooKeAPI.Logic.Email;

public interface IEmailTemplateBuilder
{
    public string GetResetPasswordTemplate(string link);
    public string GetChangeEmailTemplate(string link);
    public string GetVerifyEmailTemplate(string link);
}
