using FinBooKeApp.Core.Services.Categories.Models;
using FinBooKeApp.Core.Shared.Result;

namespace FinBooKeApp.Core.Services.Categories;

public interface ICategoryService
{
    public Task<Result<CreateCategoryResult, CategoryServiceResultCode>> CreateCategoryAsync(
        CreateCategoryParams parameters,
        CancellationToken cancellationToken
    );
    public Task<Result<GetCategoryResult, CategoryServiceResultCode>> GetCategoryAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken
    );
    public Task<Result<GetCategoriesResult, CategoryServiceResultCode>> GetCategoriesAsync(
        Guid userId,
        CancellationToken cancellationToken
    );
}
