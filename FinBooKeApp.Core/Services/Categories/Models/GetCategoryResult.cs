using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Core.Services.Categories.Models;

public record GetCategoryResult
{
    public required Category Category { init; get; }
    public List<CategoryLimit> CategoryLimit { init; get; } = [];
}
