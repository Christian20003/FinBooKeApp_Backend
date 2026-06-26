using System.ComponentModel.DataAnnotations;

namespace FinBooKeAPI.Models.DTO.Authentication;

public record ResetPasswordTokenDTO
{
    [EmailAddress(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Email),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required string Email { init; get; }
}
