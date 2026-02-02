using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.CategoryType;

public partial class CategoryService : ICategoryService
{
    public IEnumerable<CategoryNested> NestCategories(IEnumerable<Category> categories)
    {
        LogNestCategories(categories);
        var result = new List<CategoryNested>();
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
    /// <see cref="CategoryNested"/> object recursively.
    /// </summary>
    /// <param name="category">
    /// The category that should be transformed.
    /// </param>
    /// <param name="categories">
    /// A dictionary of categories that could be a child.
    /// </param>
    /// <returns>
    /// A <see cref="CategoryNested"/> object.
    /// </returns>
    private CategoryNested TransformCategory(
        Category category,
        Dictionary<Guid, Category> categories
    )
    {
        LogTransform(category);
        var result = new CategoryNested(category);
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
        Message = "CategoryType: Nest categories - {Categories}"
    )]
    private partial void LogNestCategories(IEnumerable<Category> categories);

    [LoggerMessage(
        EventId = LogEvents.CategoryNestSuccess,
        Level = LogLevel.Information,
        Message = "CategoryType: Nested categories successfully - {Categories}"
    )]
    private partial void LogCategoriesNested(IEnumerable<CategoryNested> categories);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "CategoryType: Transform category - {Category}"
    )]
    private partial void LogTransform(Category category);
}
