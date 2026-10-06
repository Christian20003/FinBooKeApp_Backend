using FinBooKeApp.Api.Shared.Attributes;
using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Api.Controllers.Profile.Models;

public record SetThemeDTO
{
    [EnumRange<ThemeType>(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Theme),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required ThemeType ThemeType { init; get; }
}
