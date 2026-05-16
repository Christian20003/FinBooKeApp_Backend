namespace FinBooKeAPI.Logic.Email;

public interface IEmailTemplateBuilder
{
    public string GetResetPasswordTemplate(string link);
}
