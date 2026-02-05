using System.ComponentModel.DataAnnotations;

namespace FinBookeAPI.Models.Category;

/// <summary>
/// This class represents a category limit.
/// </summary>
public class Limit
{
    [Required(ErrorMessage = "Limit amount is missing")]
    [Range(1d, double.MaxValue, ErrorMessage = "Limit amount must be between {1} and {2}")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Limit period is missing")]
    [Range(1, int.MaxValue, ErrorMessage = "Limit period mus be between {1} and {2}")]
    public int PeriodDays { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public Limit Copy()
    {
        return new Limit
        {
            Amount = Amount,
            PeriodDays = PeriodDays,
            CreatedAt = CreatedAt,
            ModifiedAt = ModifiedAt,
        };
    }

    public override string ToString()
    {
        return $"Limit: {{ Amount: {Amount}, PeriodDays: {PeriodDays}, CreatedAt: {CreatedAt}, ModifiedAt: {ModifiedAt} }}";
    }
}
