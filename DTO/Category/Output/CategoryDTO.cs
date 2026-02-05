using FinBookeAPI.Models.Category;

namespace FinBookeAPI.DTO.Category.Output;

/// <summary>
/// This record represents the response message of a category request.
/// </summary>
public record CategoryDTO : CategoryBaseDTO
{
    public Guid Id { get; set; }

    public new string Name => base.Name!;

    public new string Color => base.Color!;

    public new IEnumerable<Guid> Children { get; set; } = [];

    public new LimitDTO? Limit => base.Limit as LimitDTO;

    public string? Url { get; set; } = null;

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public CategoryDTO(CategoryTag category, string? url)
    {
        Id = category.Id;
        base.Name = category.Name;
        base.Color = category.Color;
        Children = category.Children;
        base.Limit = category.Limit is not null ? new LimitDTO(category.Limit) : null;
        Url = url;
        CreatedAt = category.CreatedAt;
        ModifiedAt = category.ModifiedAt;
    }
}
