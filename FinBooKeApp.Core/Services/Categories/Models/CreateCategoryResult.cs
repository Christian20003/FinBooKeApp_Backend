using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Core.Services.Categories.Models;

public record CreateCategoryResult
{
    public required Category CreatedCategory { init; get; }
    public CategoryLimit? CreatedLimit { init; get; } = null;
    public List<Category> ModifiedCategories { init; get; } = [];
    public List<CategoryLimit> ModifiedCategoryLimits { init; get; } = [];
}
