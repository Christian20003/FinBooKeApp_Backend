using System.ComponentModel.DataAnnotations;

namespace FinBooKeAPI.Models.DTO.Profile;

public record SetUsernameDTO
{
    [Required(
        ErrorMessageResourceName = nameof(DataAnnotationValidation.Username),
        ErrorMessageResourceType = typeof(DataAnnotationValidation)
    )]
    public required string Username { init; get; }
}
