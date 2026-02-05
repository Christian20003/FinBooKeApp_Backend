using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.Category;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.Category;

public partial class CategoryService : ICategoryService
{
    /// <summary>
    /// This method verifies the properties of a category.
    /// </summary>
    /// <param name="category">
    /// The category that should be verified.
    /// </param>
    /// <exception cref="ValidationException">
    /// If a property does not fulfill the requirements.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If a child category does not exist.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If a child category cannot be accessed by the user.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If a read operation on the database fails.
    /// </exception>
    private async Task VerifyCategory(CategoryTag category)
    {
        LogVerifyCategory(category);
        var validator = new ValidationContext(category);
        Validator.ValidateObject(category, validator, true);

        var children = await _collection.GetCategories(obj => category.Children.Contains(obj.Id));

        VerifyCategoryChildren(category, children);
        await VerifyCategoryLimit(category, children);
    }

    /// <summary>
    /// This method verifies the properties and the access
    /// of a category.
    /// </summary>
    /// <param name="category">
    /// The category that should be verifed.
    /// </param>
    /// <returns>
    /// The category object from the database.
    /// </returns>
    /// <exception cref="ValidationException">
    /// If a property does not fulfill the requirements.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If the category does not exist.
    /// If a child of the category does not exist.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the category cannot be accessed by the user.
    /// If a child category cannot be accessed by the user.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If a reading operation has been canceled.
    /// </exception>
    private async Task<CategoryTag> VerifyExistingCategory(CategoryTag category)
    {
        LogVerifyExistingCategory(category);
        await VerifyCategory(category);
        return await VerifyCategoryAccess(category.Id, category.UserId);
    }

    /// <summary>
    /// This method verifies if the user has access on a
    /// category.
    /// </summary>
    /// <param name="categoryId">
    /// The id of the category to which the user wants acccess.
    /// </param>
    /// <param name="userId">
    /// The id of the user.
    /// </param>
    /// <returns>
    /// The category object from the database.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the provided category id is an empty GUID.
    /// If the provided user id is an empty GUID.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If the database object could not be found.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the user does not have access to the category.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the reading operation has been canceled.
    /// </exception>
    private async Task<CategoryTag> VerifyCategoryAccess(Guid categoryId, Guid userId)
    {
        LogVerifyCategoryAccess(categoryId, userId);
        if (categoryId == Guid.Empty)
        {
            LogInvalidCategoryId(categoryId);
            throw new ArgumentException("Category id is invalid", nameof(categoryId));
        }
        if (userId == Guid.Empty)
        {
            LogInvalidUserId(userId);
            throw new ArgumentException("UserId of category is invalid", nameof(userId));
        }
        var entity = await _collection.GetCategory(category => category.Id == categoryId);
        if (entity is null)
        {
            LogNotFound(categoryId);
            throw new EntityNotFoundException("Category does not exist");
        }
        if (entity.UserId != userId)
        {
            LogNotAccessible(categoryId);
            throw new AuthorizationException("Category is not accessible");
        }
        return entity;
    }

    /// <summary>
    /// This method verifies the children of a category.
    /// </summary>
    /// <param name="parent">
    /// The category that should be verified.
    /// </param>
    /// <param name="children">
    /// A list of categories that are the children of the
    /// verfied category.
    /// </param>
    /// <exception cref="EntityNotFoundException">
    /// If the database object of a child could not be found.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the user does not have access to a child.
    /// </exception>
    private void VerifyCategoryChildren(CategoryTag parent, IEnumerable<CategoryTag> children)
    {
        LogVerifyCategoryChildren(parent, children);
        var missing = parent.Children.Where(id => !children.Any(child => child.Id == id));
        var inaccessable = children.Where(child => child.UserId != parent.UserId);
        if (missing.Any())
        {
            LogNotFound(missing.First());
            throw new EntityNotFoundException("Category children does not exist");
        }
        if (inaccessable.Any())
        {
            LogNotAccessible(inaccessable.First().Id);
            throw new AuthorizationException("Category children is not accessible");
        }
    }

    /// <summary>
    /// This method verifies the limit of a category.
    /// </summary>
    /// <param name="category">
    /// The category thats limit should be verified.
    /// </param>
    /// <param name="children">
    /// A list of categories that are the children of the
    /// verfied category.
    /// </param>
    /// <exception cref="ValidationException">
    /// If the limit amount is smaller or equal to zero.
    /// If the limit amount is larger than that of its potential parent.
    /// If the limit amount is smaller than the sum of its potential children.
    /// If the limit period is smaller or equal to zero.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the reading operation has been canceled.
    /// </exception>
    private async Task VerifyCategoryLimit(CategoryTag category, IEnumerable<CategoryTag> children)
    {
        LogVerifyCategoryLimit(category);
        if (category.Limit is null)
            return;
        var parent = await _collection.GetCategory(obj => obj.Children.Contains(category.Id));
        var sum = children.Aggregate<CategoryTag, decimal>(
            0,
            (sum, cat) => sum += cat.Limit?.Amount ?? 0
        );
        var limit = category.Limit;
        if (limit.Amount <= 0)
        {
            LogInvalidLimit(limit);
            throw new ValidationException("Limit amount must be larger than zero");
        }
        if (parent is not null && parent.Limit!.Amount < limit.Amount)
        {
            LogInvalidLimit(limit);
            throw new ValidationException(
                "Limit amount must be smaller than the amount of the parent"
            );
        }
        if (sum != 0 && limit.Amount < sum)
        {
            LogInvalidLimit(limit);
            throw new ValidationException(
                "Limit amount must be larger than the amount of its children"
            );
        }
        if (limit.PeriodDays <= 0)
        {
            LogInvalidLimit(limit);
            throw new ValidationException("Limit period must be larger than zero");
        }
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Category: Validate category - Category: {Category}"
    )]
    private partial void LogVerifyCategory(CategoryTag category);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Category: Validate existing category - Category: {Category}"
    )]
    private partial void LogVerifyExistingCategory(CategoryTag category);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Category: Validate category access - CategoryId: {Id}, UserId: {UserId}"
    )]
    private partial void LogVerifyCategoryAccess(Guid id, Guid userId);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Category: Validate category children - Parent: {Parent}, Children: {Children}"
    )]
    private partial void LogVerifyCategoryChildren(
        CategoryTag parent,
        IEnumerable<CategoryTag> children
    );

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Category: Validate category limit - Category: {Category}"
    )]
    private partial void LogVerifyCategoryLimit(CategoryTag category);

    [LoggerMessage(
        EventId = LogEvents.CategoryInvalidId,
        Level = LogLevel.Error,
        Message = "CategoryType: Category id is invalid - Id: {Id}"
    )]
    private partial void LogInvalidCategoryId(Guid id);

    [LoggerMessage(
        EventId = LogEvents.CategoryNotFound,
        Level = LogLevel.Error,
        Message = "Category: Category does not exist - {Id}"
    )]
    private partial void LogNotFound(Guid id);

    [LoggerMessage(
        EventId = LogEvents.CategoryInvalidLimit,
        Level = LogLevel.Error,
        Message = "Category: Category limit is invalid - {Limit}"
    )]
    private partial void LogInvalidLimit(Limit limit);
}
