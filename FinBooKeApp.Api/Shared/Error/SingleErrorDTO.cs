namespace FinBooKeApp.Api.Shared.Error;

public record SingleErrorDTO : BaseErrorDTO
{
    public required string Error { get; set; }
}
