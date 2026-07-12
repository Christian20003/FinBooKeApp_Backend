namespace FinBooKeAPI.Models.DTO.Error;

public record MultipleErrorDTO : BaseErrorDTO
{
    public required Dictionary<string, string[]> Errors { get; init; }
}
