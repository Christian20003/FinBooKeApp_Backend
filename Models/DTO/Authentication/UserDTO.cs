namespace FinBooKeAPI.Models.DTO.Authentication;

public record UserDTO
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string ImagePath { get; init; }
    public required SessionDTO Session { get; init; }
}
