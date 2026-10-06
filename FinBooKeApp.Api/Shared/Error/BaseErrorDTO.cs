namespace FinBooKeApp.Api.Shared.Error;

public abstract record BaseErrorDTO
{
    public required string Type { get; set; }
    public required string Title { get; set; }
    public required int Status { get; set; }
    public required string TraceId { get; set; }
}
