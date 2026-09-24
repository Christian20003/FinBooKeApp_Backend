using FinBookeAPI.Attributes;
using FinBooKeAPI.Models.Database.Account;

namespace FinBooKeAPI.Models.DTO.Profile;

public record SetThemeDTO
{
    [EnumRange<ThemeType>(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Theme),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required ThemeType ThemeType { init; get; }
}
