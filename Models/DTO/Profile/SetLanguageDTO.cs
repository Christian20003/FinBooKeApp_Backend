using FinBookeAPI.Attributes;
using FinBooKeAPI.Models.Database.Account;

namespace FinBooKeAPI.Models.DTO.Profile;

public record SetLanguageDTO
{
    [EnumRange<LanguageType>(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Language),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required LanguageType LanguageType { init; get; }
}
