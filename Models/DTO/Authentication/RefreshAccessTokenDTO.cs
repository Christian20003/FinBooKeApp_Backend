using System.ComponentModel.DataAnnotations;

namespace FinBooKeAPI.Models.DTO.Authentication;

public record RefreshAccessTokenDTO
{
    [Required(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.RefreshToken),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required string RefreshToken { get; init; }

    [EmailAddress(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Email),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required string Email { get; init; }
}
