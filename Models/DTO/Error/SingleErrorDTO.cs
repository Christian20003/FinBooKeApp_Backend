namespace FinBooKeAPI.Models.DTO.Error;

public record SingleErrorDTO : BaseErrorDTO
{
    public required string Error { get; init; }
}
