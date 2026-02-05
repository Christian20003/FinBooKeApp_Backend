using FinBookeAPI.Attributes;
using FinBookeAPI.Models.Category;

namespace FinBookeAPI.DTO.Category;

/// <summary>
/// This class respresents the base class of a category request.
/// </summary>
public record CategoryBaseDTO
{
    public string? Name { get; set; }

    [Color(ErrorMessage = "Given color is not a valid color encoding")]
    public string? Color { get; set; }

    [Guid(ErrorMessage = "Given children id is not a valid GUID")]
    public IEnumerable<string>? Children { get; set; }

    public LimitBaseDTO? Limit { get; set; }

    public CategoryTag GetCategory(Guid userId)
    {
        return new CategoryTag
        {
            Name = Name ?? "",
            Color = Color ?? "",
            UserId = userId,
            Children = Children?.Select(Guid.Parse).ToList() ?? [],
            Limit = Limit?.GetLimit(),
        };
    }
}
