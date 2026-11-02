using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Data.Interfaces;

public interface ICategoryLimitCollection
{
    public Task<List<CategoryLimit>> SelectByCategoryId(
        Guid categoryId,
        CancellationToken cancellationToken
    );
    public Task<List<CategoryLimit>> SelectByCategoryId(
        List<Guid> categoryIds,
        CancellationToken cancellationToken
    );
    public Task<CategoryLimit?> SelectCurrentLimitByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken
    );
    public void Update(CategoryLimit limit);
    public void Insert(CategoryLimit limit);
    public Task SaveAsync(CancellationToken cancellationToken);
}
