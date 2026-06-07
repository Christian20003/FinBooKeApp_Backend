namespace FinBooKeAPI.Models.DTO.Error;

public record BadRequestDTO
{
    public required string Type { get; init; }
    public required string Title { get; init; }
    public required int Status { get; init; }
    public required Dictionary<string, string[]> Errors { get; init; }
    public required string TraceId { get; init; }
}
