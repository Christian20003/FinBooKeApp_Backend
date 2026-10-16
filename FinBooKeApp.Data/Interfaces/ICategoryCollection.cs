using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Data.Interfaces;

public interface ICategoryCollection
{
    public Task InsertAsync(Category category, CancellationToken cancellationToken);
    public Task UpdateAsync(Category category, CancellationToken cancellationToken);
    public Task DeleteAsync(Category category, CancellationToken cancellationToken);
    public Task<Category?> SelectAsync(Guid id, CancellationToken cancellationToken);
}
