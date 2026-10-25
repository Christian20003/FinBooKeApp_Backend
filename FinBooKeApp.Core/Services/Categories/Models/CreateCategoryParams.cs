using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Core.Services.Categories.Models;

public record CreateCategoryParams
{
    public required Guid UserId { get; init; }
    public required string Name { get; init; }
    public required string Color { get; init; }
    public List<Guid>? ChildrenIds { get; init; } = null;
    public Guid? ParentId { get; init; } = null;
    public decimal? LimitAmount { get; init; } = null;
    public DateTime? LimitStartDate { get; init; } = null;
    public TimePeriod? LimitTimePeriod { get; init; } = null;
}
