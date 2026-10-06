using System.ComponentModel.DataAnnotations;

namespace FinBooKeApp.Api.Controllers.Profile.Models;

public record SetUsernameDTO
{
    [Required(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Username),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required string Username { init; get; }
}
