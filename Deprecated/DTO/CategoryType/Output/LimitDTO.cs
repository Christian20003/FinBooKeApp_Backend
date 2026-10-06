using FinBookeAPI.Models.CategoryType;

namespace FinBookeAPI.DTO.CategoryType.Output;

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
