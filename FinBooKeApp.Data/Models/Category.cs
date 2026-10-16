namespace FinBooKeApp.Data.Models;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid UserId { get; set; }
    public required string Name { get; set; }
    public required string Color { get; set; }
    public IEnumerable<Guid> Children { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}

public class CategoryLimit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid CategoryId { get; set; }
    public required decimal Amount { get; set; }
    public required TimePeriod Period { get; set; }
    public required DateTime Start { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}

public enum TimePeriod
{
    ONE_MONTH,
    TWO_MONTHS,
    THREE_MONTHS,
    FOUR_MONTHS,
    FIVE_MONTHS,
    SIX_MONTHS,
    SEVEN_MONTHS,
    EIGHT_MONTHS,
    NINE_MONTHS,
    TEN_MONTHS,
    ELEVEN_MONTHS,
    TWELVE_MONTHS,
}
