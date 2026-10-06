using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FinBookeAPI.Services.CategoryType;

public interface ICategoryService
{
    /// <summary>
    /// This method creates a category in the database.
    /// </summary>
    /// <param name="category">
    /// The category that should be added.
    /// </param>
    /// <returns>
    /// The category that has been added to the database.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the category name or color is null or empty.
    /// If the category id is an empty GUID
    /// If the category userId is an empty GUID.
    /// If the category limit amount is smaller or equal to zero.
    /// If the category limit amount is larger than that of its potential parent.
    /// If the category limit amount is smaller than the sum of its potential children.
    /// If the category limit period is smaller or equal to zero.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If a category children does not exist in the database.
    /// </exception>
    /// <exception cref="FormatException">
    /// If the category color is not a valid color format.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the user id of a child category is different.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If tracking operations have been canceled.
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// If the category collection could not be updated.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    /// If the category collection could not be updated due to concurrency issues.
    /// </exception>
    public Task<Category> CreateCategory(Category category);

    /// <summary>
    /// This method returns all categories that corresponds to the user.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// All categories from a user.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the user id is an empty GUID.
    /// </exception>
    public Task<IEnumerable<Category>> GetCategories(Guid userId);

    /// <summary>
    /// This method returns a category.
    /// </summary>
    /// <param name="categoryId">
    /// The id of the requested category.
    /// </param>
    /// <param name="userId">
    /// The id of the user who wants access.
    /// </param>
    /// <returns>
    /// The category from the database.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the category id is an empty GUID.
    /// If the user id is an empty GUID.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If the category does not exist in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the category in the database has a different user id.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If tracking operations have been canceled.
    /// </exception>
    public Task<Category> GetCategory(Guid categoryId, Guid userId);

    /// <summary>
    /// This method updates a category. If this category update
    /// includes children that are already assigned to different
    /// categories, the old parent categories are updated as well.
    /// </summary>
    /// <param name="category">
    /// The category that should be updated.
    /// </param>
    /// <returns>
    /// A collection of updated categories.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the category name or color is null or empty.
    /// If the category id is an empty GUID.
    /// If the category userId is an empty GUID.
    /// If the category limit amount is smaller or equal to zero.
    /// If the category limit amount is larger than that of its potential parent.
    /// If the category limit amount is smaller than the sum of its potential children.
    /// If the category limit period is smaller or equal to zero.
    /// If a cyclic dependency exists with the updated children.
    /// </exception>
    /// <exception cref="FormatException">
    /// If the category color is not a valid color format.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If the category does not exist in the database.
    /// If a category children does not exist in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If a child has a different user id.
    /// If the category in the database has a different user id.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If tracking operations have been canceled.
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// If the category collection could not be updated.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    /// If the category collection could not be updated due to concurrency issues.
    /// </exception>
    public Task<IEnumerable<Category>> UpdateCategory(Category category);

    /// <summary>
    /// This method removes a category from the database.
    /// </summary>
    /// <param name="categoryId">
    /// The id of the category that should be removed.
    /// </param>
    /// <param name="userId">
    /// The id of the user.
    /// </param>
    /// <returns>
    /// The removed category.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the category id is an empty GUID.
    /// If the user id is an empty GUID.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If the category does not exist in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the category in the database has a different user id.
    /// If a potential parent category has a different user id.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If tracking operations have been canceled.
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// If the category collection could not be updated.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    /// If the category collection could not be updated due to concurrency issues.
    /// </exception>
    public Task<Category> DeleteCategory(Guid categoryId, Guid userId);

    /// <summary>
    /// This method transforms a simple category list into a list of nested
    /// categories.
    /// </summary>
    /// <param name="categories">
    /// The list of categories that should be nested.
    /// </param>
    /// <returns>
    /// All categories in a nested structure.
    /// </returns>
    public IEnumerable<CategoryNested> NestCategories(IEnumerable<Category> categories);
}
