namespace FinBooKeApp.Core.Shared.Authentication.Models;

public record AuthenticationToken
{
    public string Value { get; set; } = "";
    public long Expires { get; set; }
}
