using System.ComponentModel.DataAnnotations;

namespace FinBooKeApp.Api.Controllers.Authentication.Models;

public record LoginDTO
{
    [EmailAddress(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Email),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public string Email { get; init; } = string.Empty;

    [Required(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Password),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public string Password { get; init; } = string.Empty;
}
