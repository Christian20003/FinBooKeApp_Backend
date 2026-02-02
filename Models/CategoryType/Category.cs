namespace FinBookeAPI.Models.CategoryType;

/// <summary>
/// This class models a single category
/// </summary>
public class Category
{
    /// <summary>
    /// The id of the category. If not set a new id will be generated.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The id of the user who created the category.
    /// </summary>
    public Guid UserId { get; set; } = Guid.Empty;

    /// <summary>
    /// The name of the category.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The color of the category (in hex, rgb, cmyk or hsl encoding)
    /// </summary>
    public string Color { get; set; } = "";

    /// <summary>
    /// The amount limit.
    /// </summary>
    public Limit? Limit { get; set; }

    /// <summary>
    /// The ids of its sub-categories
    /// </summary>
    public IEnumerable<Guid> Children { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public Category() { }

    public Category(Category category)
    {
        Id = category.Id;
        Name = category.Name;
        UserId = category.UserId;
        Color = category.Color;
        Children = category.Children;
        if (category.Limit is not null)
            Limit = new Limit(category.Limit);
        CreatedAt = category.CreatedAt;
        ModifiedAt = category.ModifiedAt;
    }

    public override string ToString()
    {
        return $"Category: {{ Id: {Id}, Name: {Name}, UserId: {UserId}, Limit: {Limit?.ToString()}, Children: [{string.Join(", ", Children)}], Color: {Color}, CreatedAt: {CreatedAt}, ModifiedAt: {ModifiedAt} }}";
    }
}
