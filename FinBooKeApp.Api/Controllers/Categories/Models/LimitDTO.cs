using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Api.Controllers.Categories.Models;

public record LimitDTO
{
    public Guid Id { init; get; }
    public decimal Amount { init; get; }
    public DateTime Start { init; get; }
    public TimePeriod MonthlyDuration { init; get; }
    public DateTime CreatedAt { init; get; }
    public DateTime ModifiedAt { init; get; }

    public LimitDTO(CategoryLimit limit)
    {
        Id = limit.Id;
        Amount = limit.Amount;
        Start = limit.Start;
        MonthlyDuration = limit.Period;
        CreatedAt = limit.CreatedAt;
        ModifiedAt = limit.ModifiedAt;
    }
}
