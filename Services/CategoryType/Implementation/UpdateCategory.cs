using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.CategoryType;

public partial class CategoryService : ICategoryService
{
    public async Task<IEnumerable<Category>> UpdateCategory(Category category)
    {
        LogUpdateCategory(category);

        var entity = await VerifyExistingCategory(category);
        var result = new List<Category> { new(category) };

        // Set new properties if they have changed
        if (entity.Name != category.Name)
            entity.Name = category.Name;
        if (entity.Color != category.Color)
            entity.Color = category.Color;
        if (entity.Limit != category.Limit)
        {
            if (category.Limit is null)
                entity.Limit = null;
            else if (entity.Limit is null)
                entity.Limit = category.Limit;
            else
            {
                entity.Limit.Amount = category.Limit.Amount;
                entity.Limit.PeriodDays = category.Limit.PeriodDays;
                entity.ModifiedAt = DateTime.UtcNow;
            }
        }

        if (!entity.Children.Equals(category.Children))
        {
            // Update old parents of new childs
            var addedChilds = category.Children.Where(childId =>
                !entity.Children.Contains(childId)
            );
            foreach (var child in addedChilds)
            {
                // Remove their id from children list of old parent
                var parent = await _collection.GetCategory(category =>
                    category.Children.Contains(child)
                );
                if (parent is null)
                    continue;
                if (parent.UserId != category.UserId)
                {
                    LogNotAccessibleCategory(parent);
                    throw new AuthorizationException("Category parent is not accessible");
                }
                parent.Children = [.. parent.Children.Where(childId => childId != child)];
                parent.ModifiedAt = DateTime.UtcNow;
                if (result.Any(elem => elem.Id == parent.Id))
                    result = [.. result.Where(elem => elem.Id != parent.Id)];
                result.Add(new Category(parent));
            }
            entity.Children = category.Children;

            if (await VerifyAfterCycle(category.Id, category.Children))
            {
                LogInvalidChildren(category);
                throw new ArgumentException(
                    $"Category children of {category.Id} produce a cyclic relationship",
                    nameof(category)
                );
            }
        }

        if (!category.Equals(entity))
            entity.ModifiedAt = category.ModifiedAt;

        _collection.UpdateCategory(entity);
        await _collection.SaveChanges();

        LogCategoryUpdated(result);

        return result;
    }

    [LoggerMessage(
        EventId = LogEvents.CategorUpdate,
        Level = LogLevel.Information,
        Message = "CategoryType: Update category - {Category}"
    )]
    private partial void LogUpdateCategory(Category category);

    [LoggerMessage(
        EventId = LogEvents.CategorUpdateSuccess,
        Level = LogLevel.Information,
        Message = "CategoryType: Updated category successfully - {Category}"
    )]
    private partial void LogCategoryUpdated(IEnumerable<Category> category);

    [LoggerMessage(
        EventId = LogEvents.CategoryInvalidChild,
        Level = LogLevel.Error,
        Message = "CategoryType: Category produces cyclic dependency - {Category}"
    )]
    private partial void LogInvalidChildren(Category category);
}
