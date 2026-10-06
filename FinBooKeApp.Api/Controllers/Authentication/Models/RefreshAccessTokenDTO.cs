using System.ComponentModel.DataAnnotations;

namespace FinBooKeApp.Api.Controllers.Authentication.Models;

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
