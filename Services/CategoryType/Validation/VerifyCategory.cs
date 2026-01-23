using FinBookeAPI.Attributes;
using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.CategoryType;

public partial class CategoryService : ICategoryService
{
    /// <summary>
    /// This method verifies if a existing category is valid.
    /// </summary>
    /// <param name="category">
    /// The category that should be verifed.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the category name is null or empty.
    /// If the category color is null or empty.
    /// If the limit amount is smaller or equal to zero.
    /// If the limit amount is larger than that of its potential parent.
    /// If the limit amount is smaller than the sum of its potential children.
    /// If the limit period is smaller or equal to zero.
    /// If a category child does not exist in the database.
    /// </exception>
    /// <exception cref="FormatException">
    /// If the category color is not a valid color format.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If the database object could not be found.
    /// If the database object of a child could not be found.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If a category child is not accessible by the user.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If a reading operation has been canceled.
    /// </exception>
    private async Task<Category> VerifyExistingCategory(Category category)
    {
        LogCategoryValidation(category);
        var entity = await VerifyCategoryAccess(category.Id, category.UserId);
        VerifyCategoryName(category.Name);
        VerifyCategoryColor(category.Color);
        if (category.Limit is null)
            await VerifyCategoryChildren(category.Children, category.UserId);
        else
            await VerifyCategoryLimit(
                category.Id,
                category.UserId,
                category.Limit,
                category.Children
            );
        return entity;
    }

    /// <summary>
    /// This method verifies if a new category is valid.
    /// </summary>
    /// <param name="category">
    /// The category that should be verifed.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the category name is null or empty.
    /// If the category color is null or empty.
    /// If the limit amount is smaller or equal to zero.
    /// If the limit amount is larger than that of its potential parent.
    /// If the limit amount is smaller than the sum of its potential children.
    /// If the limit period is smaller or equal to zero.
    /// If a category child does not exist in the database.
    /// </exception>
    /// <exception cref="FormatException">
    /// If the category color is not a valid color format.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If the database object of a child could not be found.
    /// </exception>
    /// <exception cref="DuplicateEntityException">
    /// If the category id does already exist in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If a category child is not accessible by the user.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If a reading operation has been canceled.
    /// </exception>
    private async Task VerifyNewCategory(Category category)
    {
        LogCategoryValidation(category);
        var entity = await VerfiyCategoryId(category.Id);
        if (entity is not null)
        {
            LogDuplicateCategory(category);
            throw new DuplicateEntityException("Category id is does already exist");
        }
        VerifyCategoryName(category.Name);
        VerifyCategoryColor(category.Color);
        if (category.Limit is null)
            await VerifyCategoryChildren(category.Children, category.UserId);
        else
            await VerifyCategoryLimit(
                category.Id,
                category.UserId,
                category.Limit,
                category.Children
            );
    }

    /// <summary>
    /// This method verifies a category id.
    /// </summary>
    /// <param name="categoryId">
    /// The category id that should be verified.
    /// </param>
    /// <returns>
    /// The category object from the database that corresponds
    /// to the provided id. If the category does not exist, this
    /// method returns <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the provided category id is an empty GUID.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the reading operation has been canceled.
    /// </exception>
    private async Task<Category?> VerfiyCategoryId(Guid categoryId)
    {
        LogCategoryIdValidation(categoryId);
        if (categoryId == Guid.Empty)
        {
            LogInvalidCategoryId(categoryId);
            throw new ArgumentException("Category id is not valid", nameof(categoryId));
        }
        return await _collection.GetCategory(category => category.Id == categoryId);
    }

    /// <summary>
    /// This method verifies if the a user has access on a
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
    private async Task<Category> VerifyCategoryAccess(Guid categoryId, Guid userId)
    {
        LogCategoryAccessValidation(categoryId);
        var entity = await VerfiyCategoryId(categoryId);
        if (userId == Guid.Empty)
        {
            LogInvalidUserId(userId);
            throw new ArgumentException("UserId of category is not valid", nameof(userId));
        }
        if (entity is null)
        {
            LogNotFoundCategory(categoryId);
            throw new EntityNotFoundException("Category does not exist");
        }
        if (entity.UserId != userId)
        {
            LogNotAccessibleCategory(entity);
            throw new AuthorizationException("Category is not accessible");
        }
        return entity;
    }

    /// <summary>
    /// This method verifies the name of a category.
    /// </summary>
    /// <param name="name">
    /// The name that should be verified.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the name is an empty string.
    /// </exception>
    private void VerifyCategoryName(string name)
    {
        LogCategoryNameValidation(name);
        if (string.IsNullOrWhiteSpace(name))
        {
            LogInvalidCategoryName(name);
            throw new ArgumentException("Category name is null or empty", nameof(name));
        }
    }

    /// <summary>
    /// This method verifies the color of a category.
    /// </summary>
    /// <param name="color">
    /// The color that should be verified.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the color is an empty string.
    /// </exception>
    /// <exception cref="FormatException">
    /// If the color is not a valid color format.
    /// </exception>
    private void VerifyCategoryColor(string color)
    {
        LogCategoryColorValidation(color);
        var colorValidator = new ColorAttribute();
        if (string.IsNullOrWhiteSpace(color))
        {
            LogInvalidCategoryColor(color);
            throw new ArgumentException("Category color is null or empty", nameof(color));
        }
        if (!colorValidator.IsValid(color))
        {
            LogInvalidCategoryColor(color);
            throw new FormatException("Category color is not a valid color encoding");
        }
    }

    /// <summary>
    /// This method verifies the category children list.
    /// </summary>
    /// <param name="childrenIds">
    /// A list containing all ids of each children.
    /// </param>
    /// <param name="userId">
    /// The id of the user.
    /// </param>
    /// <returns>
    /// A list of all category objects of each child from the
    /// database.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the provided user id is an empty GUID.
    /// If any of the provided child ids is an empty GUID.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If the database object of a child could not be found.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the user does not have access to a child.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the reading operation has been canceled.
    /// </exception>
    private async Task<IEnumerable<Category>> VerifyCategoryChildren(
        IEnumerable<Guid> childrenIds,
        Guid userId
    )
    {
        LogCategoryChildrenValidation(childrenIds);
        if (userId == Guid.Empty)
        {
            LogInvalidUserId(userId);
            throw new ArgumentException("UserId of category is not valid", nameof(userId));
        }
        if (childrenIds.Any(elem => elem == Guid.Empty))
        {
            LogInvalidCategoryId(Guid.Empty);
            throw new ArgumentException("Children id is not valid", nameof(childrenIds));
        }
        var children = await _collection.GetCategories(category =>
            childrenIds.Contains(category.Id)
        );
        var notFound = childrenIds.Where(id => !children.Any(child => child.Id == id));
        var notAccessible = children.Where(child => child.UserId != userId);
        if (notFound.Any())
        {
            LogNotFoundCategory(notFound.First());
            throw new EntityNotFoundException("Category children does not exist");
        }
        if (notAccessible.Any())
        {
            LogNotAccessibleCategory(notAccessible.First());
            throw new AuthorizationException("Category children is not accessible");
        }
        return children;
    }

    /// <summary>
    /// This method verifies the limit of a category.
    /// </summary>
    /// <param name="categoryId">
    /// The id of the category.
    /// </param>
    /// <param name="userId">
    /// The if of the user.
    /// </param>
    /// <param name="limit">
    /// The limit object if that category.
    /// </param>
    /// <param name="childrenIds">
    /// A list containing all ids of each children.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the provided user id is an empty GUID.
    /// If any of the provided child ids is an empty GUID.
    /// If the limit amount is smaller or equal to zero.
    /// If the limit amount is larger than that of its potential parent.
    /// If the limit amount is smaller than the sum of its potential children.
    /// If the limit period is smaller or equal to zero.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If the database object of a child could not be found.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the user does not have access to a child.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the reading operation has been canceled.
    /// </exception>
    private async Task VerifyCategoryLimit(
        Guid categoryId,
        Guid userId,
        Limit limit,
        IEnumerable<Guid> childrenIds
    )
    {
        LogCategoryLimitValidation(limit);
        var children = await VerifyCategoryChildren(childrenIds, userId);
        var parent = await _collection.GetCategory(category =>
            category.Children.Contains(categoryId)
        );
        var sum = children.Aggregate<Category, decimal>(
            0,
            (sum, cat) => sum += cat.Limit?.Amount ?? 0
        );
        if (limit.Amount <= 0)
        {
            LogInvalidCategoryLimit(limit);
            throw new ArgumentException("Limit amount must be larger than zero", nameof(limit));
        }
        if (parent is not null && parent.Limit!.Amount < limit.Amount)
        {
            LogInvalidCategoryLimit(limit);
            throw new ArgumentException(
                "Limit amount must be smaller than the amount of the parent",
                nameof(limit)
            );
        }
        if (sum != 0 && limit.Amount < sum)
        {
            LogInvalidCategoryLimit(limit);
            throw new ArgumentException(
                "Limit amount must be larger than the amount of its children",
                nameof(limit)
            );
        }
        if (limit.PeriodDays <= 0)
        {
            LogInvalidCategoryLimit(limit);
            throw new ArgumentException("Limit period must be larger than zero", nameof(limit));
        }
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "CategoryType: Validate category - {Category}"
    )]
    private partial void LogCategoryValidation(Category category);

    [LoggerMessage(Level = LogLevel.Trace, Message = "CategoryType: Validate category id - {Id}")]
    private partial void LogCategoryIdValidation(Guid id);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "CategoryType: Validate category access - {Id}"
    )]
    private partial void LogCategoryAccessValidation(Guid id);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "CategoryType: Validate category name - {Name}"
    )]
    private partial void LogCategoryNameValidation(string name);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "CategoryType: Validate category color - {Color}"
    )]
    private partial void LogCategoryColorValidation(string color);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "CategoryType: Validate category children - {Children}"
    )]
    private partial void LogCategoryChildrenValidation(IEnumerable<Guid> children);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "CategoryType: Validate category limit - {Limit}"
    )]
    private partial void LogCategoryLimitValidation(Limit limit);

    [LoggerMessage(
        EventId = LogEvents.CategoryDuplicate,
        Level = LogLevel.Error,
        Message = "CategoryType: Category already exist - {Category}"
    )]
    private partial void LogDuplicateCategory(Category category);

    [LoggerMessage(
        EventId = LogEvents.CategoryNotFound,
        Level = LogLevel.Error,
        Message = "CategoryType: Category does not exist - {Id}"
    )]
    private partial void LogNotFoundCategory(Guid id);

    [LoggerMessage(
        EventId = LogEvents.CategoryInvalidId,
        Level = LogLevel.Error,
        Message = "CategoryType: Category id is invalid - {Id}"
    )]
    private partial void LogInvalidCategoryId(Guid id);

    [LoggerMessage(
        EventId = LogEvents.CategoryInvalidName,
        Level = LogLevel.Error,
        Message = "CategoryType: Category name is invalid - {Name}"
    )]
    private partial void LogInvalidCategoryName(string name);

    [LoggerMessage(
        EventId = LogEvents.CategoryInvalidColor,
        Level = LogLevel.Error,
        Message = "CategoryType: Category color is invalid - {Color}"
    )]
    private partial void LogInvalidCategoryColor(string color);

    [LoggerMessage(
        EventId = LogEvents.CategoryInvalidLimit,
        Level = LogLevel.Error,
        Message = "CategoryType: Category limit is invalid - {Limit}"
    )]
    private partial void LogInvalidCategoryLimit(Limit limit);
}
