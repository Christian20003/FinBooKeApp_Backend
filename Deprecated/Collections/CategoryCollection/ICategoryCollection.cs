using FinBookeAPI.Models.CategoryType;

namespace FinBookeAPI.Collections.CategoryCollection;

public interface ICategoryCollection : IDataCollection
{
    /// <summary>
    /// This method tracks the provided category for insertion.
    /// </summary>
    /// <param name="category">
    /// The category object that should be added.
    /// </param>
    public void CreateCategory(Category category);

    /// <summary>
    /// This method tracks the provided category for updating.
    /// </summary>
    /// <param name="category">
    /// The category that should be updated.
    /// </param>
    public void UpdateCategory(Category category);

    /// <summary>
    /// This method tracks the provided category for deletion.
    /// </summary>
    /// <param name="category">
    /// The category that should be deleted.
    /// </param>
    public void DeleteCategory(Category category);

    /// <summary>
    /// This method returns the first category from the database which
    /// fulfills the defined condition.
    /// </summary>
    /// <param name="condition">
    /// The condition that a category must fulfill.
    /// </param>
    /// <returns>
    /// The requested category if it exists, othwerwise <c>null</c>.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If an operation could not be executed at the application level
    /// and has been canceled.
    /// </exception>
    public Task<Category?> GetCategory(Func<Category, bool> condition);

    /// <summary>
    /// This method returns all categories from the database which
    /// fulfill the defined condition.
    /// </summary>
    /// <param name="condition">
    /// The condition that each returned category must fulfill.
    /// </param>
    /// <returns>
    /// The requested categories.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If an operation could not be executed at the application level
    /// and has been canceled.
    /// </exception>
    public Task<IEnumerable<Category>> GetCategories(Func<Category, bool> condition);
}
