using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Api.Controllers.Categories.Models;

public record CategoryResponseDTO
{
    public CategoryDTO Category { init; get; }
    public List<CategoryDTO> ModifiedCategories { init; get; } = [];

    public CategoryResponseDTO(
        Category category,
        CategoryLimit? limit,
        List<Category> modifiedCategories,
        List<CategoryLimit> modifiedLimits
    )
    {
        Category = new CategoryDTO(category, limit is null ? [] : [limit]);
        foreach (var modifiedCategory in modifiedCategories)
        {
            var modifiedLimit = modifiedLimits.Where(item =>
                item.CategoryId == modifiedCategory.Id
            );
            ModifiedCategories.Add(new CategoryDTO(modifiedCategory, [.. modifiedLimit]));
        }
    }
}
