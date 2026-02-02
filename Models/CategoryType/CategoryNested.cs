namespace FinBookeAPI.Models.CategoryType;

/// <summary>
/// This class models a single category with nested category objects
/// in the <c>Children</c> property.
/// </summary>
public class CategoryNested : Category
{
    public new IEnumerable<CategoryNested> Children { get; set; } = [];

    public CategoryNested() { }

    public CategoryNested(Category category)
    {
        Id = category.Id;
        Name = category.Name;
        UserId = category.UserId;
        Color = category.Color;
        Limit = category.Limit;
        CreatedAt = category.CreatedAt;
        ModifiedAt = category.ModifiedAt;
    }
}
