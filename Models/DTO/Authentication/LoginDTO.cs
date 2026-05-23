using System.ComponentModel.DataAnnotations;

namespace FinBooKeAPI.Models.DTO.Authentication;

public record LoginDTO
{
    [EmailAddress(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Email),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required string Email { get; init; }

    [Required(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Password),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required string Password { get; init; }
}
