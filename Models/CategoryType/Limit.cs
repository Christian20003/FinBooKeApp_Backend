namespace FinBookeAPI.Models.CategoryType;

public class Limit
{
    /// <summary>
    /// The amount of money which can be spent.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The period during which the limit should be reset.
    /// </summary>
    public int PeriodDays { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public Limit() { }

    public Limit(Limit limit)
    {
        Amount = limit.Amount;
        PeriodDays = limit.PeriodDays;
        CreatedAt = limit.CreatedAt;
        ModifiedAt = limit.ModifiedAt;
    }

    public override string ToString()
    {
        return $"Limit: {{ Amount: {Amount}, PeriodDays: {PeriodDays}, CreatedAt: {CreatedAt}, ModifiedAt: {ModifiedAt} }}";
    }
}
