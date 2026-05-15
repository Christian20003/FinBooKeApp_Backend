using FinBooKeAPI.Models.Logic.Email;

namespace FinBooKeAPI.Logic.Email;

public interface IEmailProvider
{
    public void Send(EmailPayload payload);
}
