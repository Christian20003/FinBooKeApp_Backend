using FinBooKeApp.Api.Shared.Attributes;
using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Api.Controllers.Profile.Models;

public record SetLanguageDTO
{
    [EnumRange<LanguageType>(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Language),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required LanguageType LanguageType { init; get; }
}
