using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.Category;

namespace FinBookeAPI.DTO.Category;

/// <summary>
/// This record represents the limit specification of a category.
/// </summary>
public record LimitBaseDTO
{
    [Required(ErrorMessage = "Limit amount is missing")]
    [Range(0, double.PositiveInfinity, ErrorMessage = "Limit amount must be greater than {1}")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Limit period is missing")]
    [Range(1, int.MaxValue, ErrorMessage = "Limit period must be greater or eqaul to {1}")]
    public int PeriodDays { get; set; }

    public Limit GetLimit()
    {
        return new Limit { Amount = Amount, PeriodDays = PeriodDays };
    }
}
