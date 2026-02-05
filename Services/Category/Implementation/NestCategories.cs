using FinBookeAPI.Models.Category;
using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.Category;

public partial class CategoryService : ICategoryService
{
    public IEnumerable<CategoryLinked> NestCategories(IEnumerable<CategoryTag> categories)
    {
        LogNestCategories(categories);
        var result = new List<CategoryLinked>();
        var childIds = categories.SelectMany(elem => elem.Children);
        var mainCategories = categories.Where(elem => !childIds.Contains(elem.Id));
        var subCategories = categories
            .Where(elem => childIds.Contains(elem.Id))
            .ToDictionary(elem => elem.Id);

        foreach (var category in mainCategories)
        {
            result.Add(TransformCategory(category, subCategories));
        }
        LogCategoriesNested(result);
        return result;
    }

    /// <summary>
    /// This method transforms a <see cref="Category"/> into a
    /// <see cref="CategoryLinked"/> object recursively.
    /// </summary>
    /// <param name="category">
    /// The category that should be transformed.
    /// </param>
    /// <param name="categories">
    /// A dictionary of categories that could be a child.
    /// </param>
    /// <returns>
    /// A <see cref="CategoryLinked"/> object.
    /// </returns>
    private CategoryLinked TransformCategory(
        CategoryTag category,
        Dictionary<Guid, CategoryTag> categories
    )
    {
        LogTransform(category);
        var result = category.Transform();
        foreach (var childId in category.Children)
        {
            try
            {
                var child = TransformCategory(categories[childId], categories);
                result.Children = result.Children.Append(child);
            }
            catch (KeyNotFoundException)
            {
                continue;
            }
        }
        return result;
    }

    [LoggerMessage(
        EventId = LogEvents.CategoryNest,
        Level = LogLevel.Information,
        Message = "Category: Nest categories - Categories: {Categories}"
    )]
    private partial void LogNestCategories(IEnumerable<CategoryTag> categories);

    [LoggerMessage(
        EventId = LogEvents.CategoryNestSuccess,
        Level = LogLevel.Information,
        Message = "Category: Nested categories successfully - Categories: {Categories}"
    )]
    private partial void LogCategoriesNested(IEnumerable<CategoryLinked> categories);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Category: Transform category - Category: {Category}"
    )]
    private partial void LogTransform(CategoryTag category);
}
