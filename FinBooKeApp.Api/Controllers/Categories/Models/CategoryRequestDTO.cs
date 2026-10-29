using System.ComponentModel.DataAnnotations;
using FinBooKeApp.Api.Shared.Attributes;
using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Api.Controllers.Categories.Models;

public record CategoryRequestDTO : IValidatableObject
{
    [Required(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.MissingCategoryName),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required string Name { init; get; }

    [Required(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.MissingCategoryColor),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    [Color(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.InvalidCategoryColor),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required string Color { init; get; }

    [Guid(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.InvalidCategoryChildren),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public List<Guid> Children { init; get; } = [];

    [Guid(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.InvalidCategoryParent),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public Guid? Parent { init; get; } = null;

    public decimal? LimitAmount { init; get; } = null;

    [EnumRange<TimePeriod>(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.InvalidCategoryLimitTimePeriod),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public TimePeriod? LimitMonthlyPeriod { init; get; } = null;

    public DateTime? LimitStart { init; get; } = null;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var result = new List<ValidationResult>();
        var required = LimitAmount != null || LimitMonthlyPeriod != null || LimitStart != null;
        if (LimitAmount is null && required)
        {
            result.Add(
                new ValidationResult(
                    DataAnnotationValidation.MissingCategoryLimitAmount,
                    [nameof(LimitAmount)]
                )
            );
        }
        if (LimitMonthlyPeriod is null && required)
        {
            result.Add(
                new ValidationResult(
                    DataAnnotationValidation.MissingCategoryLimitTimePeriod,
                    [nameof(LimitMonthlyPeriod)]
                )
            );
        }
        if (LimitStart is null && required)
        {
            result.Add(
                new ValidationResult(
                    DataAnnotationValidation.MissingCategoryLimitStartDate,
                    [nameof(LimitStart)]
                )
            );
        }
        if (LimitAmount <= 0)
        {
            result.Add(
                new ValidationResult(
                    DataAnnotationValidation.InvalidCategoryLimitAmount,
                    [nameof(LimitAmount)]
                )
            );
        }
        return result;
    }
}
