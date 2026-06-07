using System.ComponentModel.DataAnnotations;

namespace FinBooKeAPI.Models.DTO.Authentication;

public record RegisterDTO
{
    [EmailAddress(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Email),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public string Email { get; init; } = string.Empty;

    [Required(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Username),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public string Username { get; init; } = string.Empty;

    [Required(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Password),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public string Password { get; init; } = string.Empty;
}
