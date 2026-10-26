using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Data.Interfaces;

public interface ICategoryLimitCollection
{
    public Task<IEnumerable<CategoryLimit>> SelectCurrentLimitByCategoryIdAsync(
        List<Guid> categoryIds,
        CancellationToken cancellationToken
    );
    public Task<CategoryLimit?> SelectCurrentLimitByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken
    );
    public void Insert(CategoryLimit limit);
    public List<CategoryLimit> GetModifiedItems();
    public Task SaveAsync(CancellationToken cancellationToken);
}
