using System.ComponentModel.DataAnnotations;

namespace FinBooKeAPI.Models.DTO.Authentication;

public record ResetPasswordDTO
{
    [EmailAddress(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Email),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required string Email { init; get; }

    [Required(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Password),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required string NewPassword { init; get; }

    [Required(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Token),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required string Token { init; get; }
}
