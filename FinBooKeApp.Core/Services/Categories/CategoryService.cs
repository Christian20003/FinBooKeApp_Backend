using FinBooKeApp.Core.Services.Categories.Models;
using FinBooKeApp.Core.Shared.Result;
using FinBooKeApp.Data.Interfaces;
using FinBooKeApp.Data.Models;
using Microsoft.Extensions.Logging;

namespace FinBooKeApp.Core.Services.Categories;

public partial class CategoryService(
    ICategoryCollection categoryCollection,
    ICategoryLimitCollection categoryLimitCollection,
    ILogger<CategoryService> logger
) : ICategoryService
{
    private readonly ICategoryCollection _categoryCollection = categoryCollection;
    private readonly ICategoryLimitCollection _categoryLimitCollection = categoryLimitCollection;
    private readonly ILogger<CategoryService> _logger = logger;

    public async Task<Result<CreateCategoryResult, CategoryServiceResultCode>> CreateCategoryAsync(
        CreateCategoryParams parameters,
        CancellationToken cancellationToken
    )
    {
        LogCreateCategory(parameters.UserId, parameters.Name);
        var (categories, limits) = await LoadDataAsync(parameters, cancellationToken);
        var validation = await ValidateCreateCategoryAsync(
            parameters,
            categories,
            limits,
            cancellationToken
        );
        if (validation != CategoryServiceResultCode.OK)
        {
            LogCreateCategoryFailure(parameters.UserId, parameters.Name, validation);
            return new Result<CreateCategoryResult, CategoryServiceResultCode>(validation);
        }
        var result = await UpdateDatabaseAsync(parameters, categories, limits, cancellationToken);
        LogCreateCategorySuccess(parameters.UserId, parameters.Name);
        return new Result<CreateCategoryResult, CategoryServiceResultCode>(result);
    }

    public async Task<Result<GetCategoryResult, CategoryServiceResultCode>> GetCategoryAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        LogGetCategory(userId, id);
        var category = await _categoryCollection.SelectAsync(id, userId, cancellationToken);
        if (category == null)
        {
            LogGetCategoryFailure(userId, id, CategoryServiceResultCode.CATEGORY_DOES_NOT_EXIST);
            return new Result<GetCategoryResult, CategoryServiceResultCode>(
                CategoryServiceResultCode.CATEGORY_DOES_NOT_EXIST
            );
        }
        var limits = await _categoryLimitCollection.SelectByCategoryId(id, cancellationToken);
        var result = new GetCategoryResult { Category = category, CategoryLimit = limits };
        LogGetCategorySuccess(userId, id);
        return new Result<GetCategoryResult, CategoryServiceResultCode>(result);
    }

    public async Task<Result<GetCategoriesResult, CategoryServiceResultCode>> GetCategoriesAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        LogGetCategories(userId);
        var categories = await _categoryCollection.SelectAsync(userId, cancellationToken);
        var ids = categories.Select(category => category.Id).ToList();
        var limits = await _categoryLimitCollection.SelectByCategoryId(ids, cancellationToken);
        var result = new GetCategoriesResult { Categories = categories, CategoryLimits = limits };
        LogGetCategoriesSuccess(userId);
        return new Result<GetCategoriesResult, CategoryServiceResultCode>(result);
    }

    private async Task<CreateCategoryResult> UpdateDatabaseAsync(
        CreateCategoryParams param,
        List<Category> categories,
        List<CategoryLimit> limits,
        CancellationToken cancellationToken
    )
    {
        var category = CreateCategory(param.Name, param.Color, param.UserId, param.ChildrenIds);
        CategoryLimit? limit = null;
        if (param.LimitAmount != null)
        {
            limit = CreateCategoryLimit(
                category.Id,
                param.LimitAmount.Value,
                param.LimitStartDate,
                param.LimitTimePeriod
            );
        }
        var parentLimit = AdjustParentLimit(param.ParentId, param.LimitAmount, limits);
        var oldParents = RemoveChildId(categories, param.ChildrenIds);
        var parent = AddChildId(category.Id, param.ParentId, categories);
        await _categoryCollection.SaveAsync(cancellationToken);
        return new CreateCategoryResult
        {
            CreatedCategory = category,
            CreatedLimit = limit,
            ModifiedCategories = parent == null ? oldParents : [.. oldParents, parent],
            ModifiedCategoryLimits = parentLimit == null ? [] : [parentLimit],
        };
    }

    private Category CreateCategory(string name, string color, Guid userId, List<Guid>? children)
    {
        var childrenIds = children ?? [];
        var category = new Category
        {
            Name = name,
            Color = color,
            UserId = userId,
            Children = childrenIds,
        };
        _categoryCollection.Insert(category);
        return category;
    }

    private CategoryLimit CreateCategoryLimit(
        Guid categoryId,
        decimal amount,
        DateTime? startDate,
        TimePeriod? monthlyPeriod
    )
    {
        var start = startDate ?? DateTime.UtcNow;
        var period = monthlyPeriod ?? TimePeriod.ONE_MONTH;
        var limit = new CategoryLimit
        {
            CategoryId = categoryId,
            Amount = amount,
            Start = start,
            Period = period,
        };
        _categoryLimitCollection.Insert(limit);
        return limit;
    }

    private List<Category> RemoveChildId(List<Category> parents, List<Guid>? childrenIds)
    {
        var result = new List<Category>();
        foreach (var id in childrenIds ?? [])
        {
            var parent = parents.FirstOrDefault(item => item.Children.Contains(id));
            if (parent is null)
                continue;
            parent.Children = parent.Children.Where(childId => childId != id);
            _categoryCollection.Update(parent);
            result.Add(parent);
        }
        return result;
    }

    private Category? AddChildId(Guid childId, Guid? parentId, List<Category> categories)
    {
        if (parentId == null)
            return null;
        var parent = categories.FirstOrDefault(category => category.Id == parentId.Value);
        if (parent == null)
            return null;
        parent.Children = [.. parent.Children, childId];
        _categoryCollection.Update(parent);
        return parent;
    }

    private CategoryLimit? AdjustParentLimit(
        Guid? parentId,
        decimal? childLimit,
        List<CategoryLimit> limits
    )
    {
        if (parentId == null || childLimit == null)
            return null;
        var parentLimit = limits.FirstOrDefault(limit => limit.CategoryId == parentId.Value);
        if (parentLimit == null)
            return null;
        if (NeedsNewLimitEntry(parentLimit))
        {
            var newLimit = CreateCategoryLimit(
                parentId.Value,
                parentLimit.Amount + childLimit.Value,
                DateTime.UtcNow,
                parentLimit.Period
            );
            _categoryLimitCollection.Insert(newLimit);
            return newLimit;
        }
        parentLimit.Amount += childLimit.Value;
        _categoryLimitCollection.Update(parentLimit);
        return parentLimit;
    }

    private static bool NeedsNewLimitEntry(CategoryLimit limit)
    {
        var now = DateTime.UtcNow;
        var period = GetLimitPeriodValue(limit.Period);
        return limit.Start.AddMonths(period) < now;
    }

    private static int GetLimitPeriodValue(TimePeriod period)
    {
        return period switch
        {
            TimePeriod.ONE_MONTH => 1,
            TimePeriod.TWO_MONTHS => 2,
            TimePeriod.THREE_MONTHS => 3,
            TimePeriod.FOUR_MONTHS => 4,
            TimePeriod.FIVE_MONTHS => 5,
            TimePeriod.SIX_MONTHS => 6,
            TimePeriod.SEVEN_MONTHS => 7,
            TimePeriod.EIGHT_MONTHS => 8,
            TimePeriod.NINE_MONTHS => 9,
            TimePeriod.TEN_MONTHS => 10,
            TimePeriod.ELEVEN_MONTHS => 11,
            TimePeriod.TWELVE_MONTHS => 12,
            _ => 0,
        };
    }

    private async Task<(List<Category>, List<CategoryLimit>)> LoadDataAsync(
        CreateCategoryParams param,
        CancellationToken cancellationToken
    )
    {
        var categories = new List<Category>();
        var limits = new List<CategoryLimit>();
        await LoadCategoryAsync(param.Name, param.UserId, categories, cancellationToken);
        await LoadCategoryAsync(param.ParentId, param.UserId, categories, cancellationToken);
        await LoadCategoryLimitAsync(param.ParentId, limits, cancellationToken);
        if (param.ChildrenIds == null)
            return (categories, limits);
        foreach (var childId in param.ChildrenIds)
        {
            await LoadCategoryAsync(childId, param.UserId, categories, cancellationToken);
            await LoadParentCategoryAsync(childId, param.UserId, categories, cancellationToken);
            await LoadCategoryLimitAsync(childId, limits, cancellationToken);
        }
        return (categories, limits);
    }

    private async Task LoadCategoryAsync(
        Guid? id,
        Guid userId,
        List<Category> categories,
        CancellationToken cancellationToken
    )
    {
        if (id == null)
            return;
        var category = await _categoryCollection.SelectAsync(id.Value, userId, cancellationToken);
        if (category != null)
            categories.Add(category);
    }

    private async Task LoadCategoryAsync(
        string name,
        Guid userId,
        List<Category> categories,
        CancellationToken cancellationToken
    )
    {
        var category = await _categoryCollection.SelectAsync(name, userId, cancellationToken);
        if (category != null)
            categories.Add(category);
    }

    private async Task LoadParentCategoryAsync(
        Guid? id,
        Guid userId,
        List<Category> categories,
        CancellationToken cancellationToken
    )
    {
        if (id == null)
            return;
        var category = await _categoryCollection.SelectParentAsync(
            id.Value,
            userId,
            cancellationToken
        );
        if (category != null)
            categories.Add(category);
    }

    private async Task LoadCategoryLimitAsync(
        Guid? categoryId,
        List<CategoryLimit> limits,
        CancellationToken cancellationToken
    )
    {
        if (categoryId == null)
            return;
        var limit = await _categoryLimitCollection.SelectCurrentLimitByCategoryIdAsync(
            categoryId.Value,
            cancellationToken
        );
        if (limit != null)
            limits.Add(limit);
    }

    private async Task<CategoryServiceResultCode> ValidateCreateCategoryAsync(
        CreateCategoryParams param,
        List<Category> categories,
        List<CategoryLimit> limits,
        CancellationToken cancellationToken
    )
    {
        if (DoesCategoryExist(param.Name, categories))
            return CategoryServiceResultCode.CATEGORY_ALREADY_EXIST;
        var result = IsParentValid(param.ParentId, categories);
        if (result != CategoryServiceResultCode.OK)
            return result;
        result = AreChildrenValid(param.ChildrenIds, categories);
        if (result != CategoryServiceResultCode.OK)
            return result;
        result = IsLimitAmountValid(param.LimitAmount, param.ChildrenIds, limits);
        if (result != CategoryServiceResultCode.OK)
            return result;
        return await ExistCycle(param.ParentId, param.ChildrenIds, param.UserId, cancellationToken);
    }

    private async Task<CategoryServiceResultCode> ExistCycle(
        Guid? parentId,
        List<Guid>? childrenIds,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        if (parentId == null || childrenIds == null)
            return CategoryServiceResultCode.OK;
        while (true)
        {
            if (childrenIds.Contains(parentId.Value))
                return CategoryServiceResultCode.INVALID_CATEGORY_DEPENDENCY;
            var category = await _categoryCollection.SelectParentAsync(
                parentId.Value,
                userId,
                cancellationToken
            );
            if (category is null)
                return CategoryServiceResultCode.OK;
            parentId = category.Id;
        }
    }

    private static CategoryServiceResultCode IsParentValid(
        Guid? parentId,
        List<Category> categories
    )
    {
        if (parentId == null)
            return CategoryServiceResultCode.OK;
        if (DoesCategoryExist(parentId.Value, categories))
            return CategoryServiceResultCode.OK;
        return CategoryServiceResultCode.PARENT_CATEGORY_DOES_NOT_EXIST;
    }

    private static CategoryServiceResultCode AreChildrenValid(
        List<Guid>? childrenIds,
        List<Category> categories
    )
    {
        if (childrenIds == null)
            return CategoryServiceResultCode.OK;
        if (ExistDuplicateId(childrenIds))
            return CategoryServiceResultCode.DUPLICATE_CHILD_ID;
        foreach (var childId in childrenIds)
        {
            if (!DoesCategoryExist(childId, categories))
                return CategoryServiceResultCode.CHILD_CATEGORY_DOES_NOT_EXIST;
        }
        return CategoryServiceResultCode.OK;
    }

    private static CategoryServiceResultCode IsLimitAmountValid(
        decimal? limitAmount,
        List<Guid>? childrenIds,
        List<CategoryLimit> limits
    )
    {
        if (limitAmount == null || childrenIds == null)
            return CategoryServiceResultCode.OK;
        var childrenLimits = limits.Where(limit => childrenIds.Contains(limit.CategoryId)).ToList();
        if (IsLimitAmountValid(limitAmount.Value, childrenLimits))
            return CategoryServiceResultCode.OK;
        return CategoryServiceResultCode.INVALID_CATEGORY_LIMIT;
    }

    private static bool IsLimitAmountValid(decimal parentLimit, List<CategoryLimit> childrenLimits)
    {
        var totalLimit = 0.0m;
        foreach (var childLimit in childrenLimits)
        {
            totalLimit += childLimit.Amount;
        }
        return totalLimit <= parentLimit;
    }

    private static bool DoesCategoryExist(string name, List<Category> categories)
    {
        return categories.Any(category => category.Name == name);
    }

    private static bool DoesCategoryExist(Guid id, List<Category> categories)
    {
        return categories.Any(category => category.Id == id);
    }

    private static bool ExistDuplicateId(List<Guid> ids)
    {
        foreach (var id in ids)
        {
            if (ids.Count(elem => elem == id) > 1)
                return true;
        }
        return false;
    }
}
