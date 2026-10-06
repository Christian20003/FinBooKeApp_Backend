namespace FinBooKeAPI.Models.Logic.Authentication;

public record AuthenticationToken
{
    public string Value { get; set; } = "";
    public long Expires { get; set; }
}
