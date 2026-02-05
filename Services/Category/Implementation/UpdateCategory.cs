using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.Category;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.Category;

public partial class CategoryService : ICategoryService
{
    public async Task<IEnumerable<CategoryTag>> UpdateCategory(CategoryTag category)
    {
        LogUpdateCategory(category);

        var entity = await VerifyExistingCategory(category);
        var result = new List<CategoryTag> { category.Copy() };

        UpdateName(entity, category.Name);
        UpdateColor(entity, category.Color);
        UpdateLimit(entity, category.Limit);
        var updates = await UpdateChildren(entity, category.Children);
        result = [.. result, .. updates];

        if (!category.Equals(entity))
            entity.ModifiedAt = category.ModifiedAt;

        _collection.UpdateCategory(entity);
        await _collection.SaveChanges();

        LogCategoryUpdated(result);

        return result;
    }

    /// <summary>
    /// This method updates the name of a category.
    /// </summary>
    /// <param name="category">
    /// The category to be updated.
    /// </param>
    /// <param name="name">
    /// The new name of the category.
    /// </param>
    private void UpdateName(CategoryTag category, string name)
    {
        LogUpdateName(category, name);
        if (category.Name != name)
            category.Name = name;
    }

    /// <summary>
    /// This method updates the color of a category.
    /// </summary>
    /// <param name="category">
    /// The category to be updated.
    /// </param>
    /// <param name="color">
    /// The new color of the category.
    /// </param>
    private void UpdateColor(CategoryTag category, string color)
    {
        LogUpdateColor(category, color);
        if (category.Color != color)
            category.Color = color;
    }

    /// <summary>
    /// This method updates the limit of a category.
    /// </summary>
    /// <param name="category">
    /// The category to be updated.
    /// </param>
    /// <param name="limit">
    /// The new limit of the category.
    /// </param>
    private void UpdateLimit(CategoryTag category, Limit? limit)
    {
        LogUpdateLimit(category, limit);
        if (category.Limit == limit)
            return;
        if (limit is null)
            category.Limit = null;
        else if (category.Limit is null)
            category.Limit = limit;
        else
        {
            category.Limit.Amount = limit.Amount;
            category.Limit.PeriodDays = limit.PeriodDays;
            category.ModifiedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// This method updates the children list of a category.
    /// </summary>
    /// <param name="category">
    /// The category to be updated.
    /// </param>
    /// <param name="children">
    /// The new children list.
    /// </param>
    /// <returns>
    /// All categories that have been updated.
    /// </returns>
    /// <exception cref="AuthorizationException">
    /// If a category cannot be accessed by the user.
    /// </exception>
    /// <exception cref="ValidationException">
    /// If the new children list produces a cyclic
    /// dependency.
    /// </exception>
    private async Task<IEnumerable<CategoryTag>> UpdateChildren(
        CategoryTag category,
        IEnumerable<Guid> children
    )
    {
        LogUpdateChildren(category, children);
        var result = new List<CategoryTag>();
        if (category.Children.Equals(children))
            return result;

        // Update old parents of new childs
        var added = children.Where(id => !category.Children.Contains(id));
        foreach (var child in added)
        {
            // Remove their id from children list of old parent
            var parent = await _collection.GetCategory(category =>
                category.Children.Contains(child)
            );
            if (parent is null)
                continue;
            if (parent.UserId != category.UserId)
            {
                LogNotAccessible(parent.Id);
                throw new AuthorizationException("Category parent is not accessible");
            }
            parent.Children = [.. parent.Children.Where(childId => childId != child)];
            parent.ModifiedAt = DateTime.UtcNow;
            if (result.Any(elem => elem.Id == parent.Id))
                result = [.. result.Where(elem => elem.Id != parent.Id)];
            result.Add(parent.Copy());
        }
        category.Children = [.. children];
        if (await VerifyAfterCycle(category.Id, category.Children))
        {
            LogInvalidChildren(category);
            throw new ValidationException(
                $"Category children of {category.Id} produce a cyclic relationship"
            );
        }
        return result;
    }

    [LoggerMessage(
        EventId = LogEvents.CategorUpdate,
        Level = LogLevel.Information,
        Message = "Category: Update category - Category: {Category}"
    )]
    private partial void LogUpdateCategory(CategoryTag category);

    [LoggerMessage(
        EventId = LogEvents.CategorUpdateSuccess,
        Level = LogLevel.Information,
        Message = "Category: Updated category successfully - Category: {Category}"
    )]
    private partial void LogCategoryUpdated(IEnumerable<CategoryTag> category);

    [LoggerMessage(
        EventId = LogEvents.CategoryInvalidChild,
        Level = LogLevel.Error,
        Message = "Category: Category produces cyclic dependency - Category: {Category}"
    )]
    private partial void LogInvalidChildren(CategoryTag category);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Category: Update category name - Category: {Category}, Name: {Name}"
    )]
    private partial void LogUpdateName(CategoryTag category, string name);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Category: Update category color - Category: {Category}, Color: {Color}"
    )]
    private partial void LogUpdateColor(CategoryTag category, string color);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Category: Update category limit - Category: {Category}, Limit: {Limit}"
    )]
    private partial void LogUpdateLimit(CategoryTag category, Limit? limit);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Category: Update category children - Category: {Category}, Children: {Children}"
    )]
    private partial void LogUpdateChildren(CategoryTag category, IEnumerable<Guid> children);
}
