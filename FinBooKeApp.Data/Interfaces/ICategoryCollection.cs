using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Data.Interfaces;

public interface ICategoryCollection
{
    public void Insert(Category category);
    public List<Category> GetModifiedItems();
    public Task SaveAsync(CancellationToken cancellationToken);
    public Task<Category?> SelectAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    public Task<IEnumerable<Category>> SelectByChildIds(
        IEnumerable<Guid> childrenIds,
        Guid userId,
        CancellationToken cancellationToken
    );
    public Task<bool> ExistsEntriesAsync(
        IEnumerable<Guid> ids,
        Guid userId,
        CancellationToken cancellationToken
    );
    public Task<bool> ExistEntryAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    public Task<bool> ExistEntryAsync(
        string name,
        Guid userId,
        CancellationToken cancellationToken
    );
}
