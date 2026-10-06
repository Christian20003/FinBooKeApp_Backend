using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Api.Controllers.Authentication.Models;

public record UserDTO
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string ImagePath { get; init; }
    public required ThemeType Theme { get; init; }
    public required LanguageType Language { get; init; }
    public required SessionDTO Session { get; init; }
}
