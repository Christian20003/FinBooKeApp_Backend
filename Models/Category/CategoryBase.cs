using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Attributes;

namespace FinBookeAPI.Models.Category;

/// <summary>
/// This class models a single category.
/// </summary>
public class CategoryBase
{
    [NonEmptyGuid(ErrorMessage = "Category id is invalid")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [NonEmptyGuid(ErrorMessage = "User id is invalid")]
    public Guid UserId { get; set; } = Guid.Empty;

    [MinLength(1, ErrorMessage = "Category name must have at least {1} character")]
    public string Name { get; set; } = "";

    [Color(ErrorMessage = "Color is not a supported color encoding")]
    public string Color { get; set; } = "";

    public Limit? Limit { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    protected T Copy<T>()
        where T : CategoryBase, new()
    {
        return new T
        {
            Id = Id,
            UserId = UserId,
            Name = Name,
            Color = Color,
            Limit = Limit?.Copy(),
            CreatedAt = CreatedAt,
            ModifiedAt = ModifiedAt,
        };
    }

    public override string ToString()
    {
        return $"{{ Id: {Id}, Name: {Name}, UserId: {UserId}, Limit: {Limit?.ToString()}, Color: {Color}, CreatedAt: {CreatedAt}, ModifiedAt: {ModifiedAt} }}";
    }
}
