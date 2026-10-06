using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Core.Services.Authentication.Models;

public record User
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string ImagePath { get; init; }
    public required ThemeType Theme { get; init; }
    public required LanguageType Language { get; init; }
    public required Session Session { get; init; }
}
