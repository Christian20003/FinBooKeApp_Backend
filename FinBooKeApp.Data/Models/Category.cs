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
    ONE_MONTH = 1,
    TWO_MONTHS = 2,
    THREE_MONTHS = 3,
    FOUR_MONTHS = 4,
    FIVE_MONTHS = 5,
    SIX_MONTHS = 6,
    SEVEN_MONTHS = 7,
    EIGHT_MONTHS = 8,
    NINE_MONTHS = 9,
    TEN_MONTHS = 10,
    ELEVEN_MONTHS = 11,
    TWELVE_MONTHS = 12,
}
