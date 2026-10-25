using FinBooKeApp.Core.Services.Categories.Models;
using FinBookeApp.Core.Shared.Result;

namespace FinBooKeApp.Core.Services.Categories;

public interface ICategoryService
{
    public Task<Result<CreateCategoryResult, CategoryServiceResultCode>> CreateCategoryAsync(
        CreateCategoryParams parameters,
        CancellationToken cancellationToken
    );
}
