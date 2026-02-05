using FinBookeAPI.Models.Category;

namespace FinBookeAPI.DTO.Category.Output;

/// <summary>
/// This record represents a part of a response in a category request.
/// </summary>
public record LimitDTO : LimitBaseDTO
{
    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public LimitDTO(Limit limit)
    {
        Amount = limit.Amount;
        PeriodDays = limit.PeriodDays;
        CreatedAt = limit.CreatedAt;
        ModifiedAt = limit.ModifiedAt;
    }
}
