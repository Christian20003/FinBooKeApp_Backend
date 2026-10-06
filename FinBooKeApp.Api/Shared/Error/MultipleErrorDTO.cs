namespace FinBooKeApp.Api.Shared.Error;

public record MultipleErrorDTO : BaseErrorDTO
{
    public required Dictionary<string, string[]> Errors { get; init; }
}
