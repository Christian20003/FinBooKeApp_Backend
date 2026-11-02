using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Core.Services.Categories.Models;

public record GetCategoriesResult
{
    public List<Category> Categories { init; get; } = [];
    public List<CategoryLimit> CategoryLimits { init; get; } = [];
}
