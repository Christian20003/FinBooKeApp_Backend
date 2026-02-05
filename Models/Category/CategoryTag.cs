using FinBookeAPI.Attributes;

namespace FinBookeAPI.Models.Category;

/// <summary>
/// This class represents a single category with ids of its
/// children.
/// </summary>
public class CategoryTag : CategoryBase
{
    [NonEmptyGuid(ErrorMessage = "Category child id is invalid")]
    public List<Guid> Children { get; set; } = [];

    public CategoryTag Copy()
    {
        var category = Copy<CategoryTag>();
        category!.Children = Children;
        return category;
    }

    public CategoryLinked Transform()
    {
        return Copy<CategoryLinked>()!;
    }

    public override string ToString()
    {
        var obj = base.ToString();
        return $"{obj[..^3]}, Children: [{string.Join(", ", Children)}]";
    }
}
