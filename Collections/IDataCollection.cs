using Microsoft.EntityFrameworkCore;

namespace FinBookeAPI.Collections;

public interface IDataCollection
{
    /// <summary>
    /// This method makes sure that all tracked changes are sent to the database
    /// for permanent storage.
    /// </summary>
    /// <exception cref="OperationCanceledException">
    /// If an operation could not be executed at the application level
    /// and has been canceled.
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// If the database collection could not be updated.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    /// If the datasbase collection could not be updated due to concurrency
    /// violations.
    /// </exception>
    public Task SaveChanges();
}
