namespace FinBooKeAPI.Models.DTO.Error;

public record FailedRequestDTO
{
    public required string Type { get; init; }
    public required string Title { get; init; }
    public required int Status { get; init; }
    public required string Error { get; init; }
    public required string TraceId { get; init; }
}
