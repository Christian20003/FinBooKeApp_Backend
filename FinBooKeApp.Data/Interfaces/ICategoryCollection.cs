using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Data.Interfaces;

public interface ICategoryCollection
{
    public void Insert(Category category);
    public Task SaveAsync(CancellationToken cancellationToken);
    public Task<Category?> SelectAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    public Task<Category?> SelectAsync(
        string name,
        Guid userId,
        CancellationToken cancellationToken
    );
    public Task<Category?> SelectParentAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken
    );
}
