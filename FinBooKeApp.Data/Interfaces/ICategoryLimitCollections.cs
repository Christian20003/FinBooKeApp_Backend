using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Data.Interfaces;

public interface ICategoryLimitCollection
{
    public Task<CategoryLimit?> SelectCurrentLimitByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken
    );
    public void Insert(CategoryLimit limit);
    public Task SaveAsync(CancellationToken cancellationToken);
}
