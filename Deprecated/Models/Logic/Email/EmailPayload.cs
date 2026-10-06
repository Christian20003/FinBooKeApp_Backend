namespace FinBooKeAPI.Models.Logic.Email;

public record EmailPayload
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string From { get; set; }
    public required IEnumerable<string> To { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public required bool IsHtml { get; set; } = true;
    public required string Username { get; set; }
    public required string Password { get; set; }
}
