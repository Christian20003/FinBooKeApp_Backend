using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Api.Controllers.Categories.Models;

public record CategoryDTO
{
    public Guid Id { init; get; }
    public string Name { init; get; }
    public string Color { init; get; }
    public List<Guid> Children { get; init; } = [];
    public List<LimitDTO> Limits { get; init; } = [];
    public DateTime CreatedAt { init; get; }
    public DateTime ModifiedAt { init; get; }

    public CategoryDTO(Category category, List<CategoryLimit> limits)
    {
        Id = category.Id;
        Name = category.Name;
        Color = category.Color;
        Children = [.. category.Children];
        CreatedAt = category.CreatedAt;
        ModifiedAt = category.ModifiedAt;

        foreach (var limit in limits)
        {
            Limits.Add(new LimitDTO(limit));
        }
    }
}
