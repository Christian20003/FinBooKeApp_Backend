using FinBookeAPI.Models.CategoryType;
using Microsoft.AspNetCore.Mvc;

namespace FinBookeAPI.DTO.CategoryType.Output;

public record CategoryNestedDTO : CategoryBaseDTO
{
    public Guid Id { get; set; }

    public new string Name => base.Name!;

    public new string Color => base.Color!;

    public new IEnumerable<CategoryNestedDTO> Children { get; set; } = [];

    public new LimitDTO? Limit => base.Limit as LimitDTO;

    public string Url { get; set; } = "";

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public CategoryNestedDTO(CategoryNested category, string url, IUrlHelper helper)
    {
        Id = category.Id;
        base.Name = category.Name;
        base.Color = category.Color;
        base.Limit = category.Limit is not null ? new LimitDTO(category.Limit) : null;
        Url = $"{url}{helper.Action(nameof(GetCategory), new { id = category.Id })}";
        CreatedAt = category.CreatedAt;
        ModifiedAt = category.ModifiedAt;

        foreach (var child in category.Children)
        {
            Children = [.. Children, new CategoryNestedDTO(child, url, helper)];
        }
    }
}
