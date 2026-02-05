namespace FinBookeAPI.Models.Category;

/// <summary>
/// This class represents a single category with all its
/// children as category objects.
/// </summary>
public class CategoryLinked : CategoryBase
{
    public IEnumerable<CategoryLinked> Children { get; set; } = [];

    public CategoryLinked Copy()
    {
        var category = Copy<CategoryLinked>();
        category!.Children = Children.Select(child => child.Copy());
        return category;
    }

    public override string ToString()
    {
        var obj = base.ToString();
        return $"{obj[..^3]}, Children: [{string.Join(", ", Children.ToString())}]";
    }
}
