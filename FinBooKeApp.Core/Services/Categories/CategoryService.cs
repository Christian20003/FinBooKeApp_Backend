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
        var newCategory = new Category
        {
            UserId = parameters.UserId,
            Name = parameters.Name,
            Color = parameters.Color,
        };
        if (
            await _categoryCollection.ExistEntryAsync(
                parameters.Name,
                parameters.UserId,
                cancellationToken
            )
        )
        {
            LogCreateCategoryFailure(
                parameters.UserId,
                parameters.Name,
                CategoryServiceResultCode.CATEGORY_ALREADY_EXIST
            );
            return new Result<CreateCategoryResult, CategoryServiceResultCode>(
                CategoryServiceResultCode.CATEGORY_ALREADY_EXIST
            );
        }
        if (parameters.ParentId is not null && parameters.ChildrenIds is not null)
        {
            if (
                await ExistCycle(
                    parameters.ParentId.Value,
                    parameters.UserId,
                    parameters.ChildrenIds,
                    cancellationToken
                )
            )
            {
                LogCreateCategoryFailure(
                    parameters.UserId,
                    parameters.Name,
                    CategoryServiceResultCode.INVALID_CATEGORY_DEPENDENCY
                );
                return new Result<CreateCategoryResult, CategoryServiceResultCode>(
                    CategoryServiceResultCode.INVALID_CATEGORY_DEPENDENCY
                );
            }
        }
        if (parameters.ParentId is not null)
        {
            var code = await UpdateParentCategoryAsync(
                parameters.ParentId.Value,
                newCategory.Id,
                parameters.UserId,
                parameters.LimitAmount,
                cancellationToken
            );
            if (code != CategoryServiceResultCode.OK)
            {
                LogCreateCategoryFailure(parameters.UserId, parameters.Name, code);
                return new Result<CreateCategoryResult, CategoryServiceResultCode>(code);
            }
        }
        if (parameters.ChildrenIds is not null)
        {
            var code = await UpdateChildCategoriesAsync(
                parameters.ChildrenIds,
                parameters.UserId,
                parameters.LimitAmount,
                cancellationToken
            );
            if (code != CategoryServiceResultCode.OK)
            {
                LogCreateCategoryFailure(parameters.UserId, parameters.Name, code);
                return new Result<CreateCategoryResult, CategoryServiceResultCode>(code);
            }
            newCategory.Children = parameters.ChildrenIds;
        }
        Guid? limitId = null;
        if (parameters.LimitAmount is not null)
        {
            var newLimit = new CategoryLimit
            {
                CategoryId = newCategory.Id,
                Amount = parameters.LimitAmount.Value,
                Start = parameters.LimitStartDate is not null
                    ? parameters.LimitStartDate.Value
                    : DateTime.UtcNow,
                Period = parameters.LimitTimePeriod is not null
                    ? parameters.LimitTimePeriod.Value
                    : TimePeriod.ONE_MONTH,
            };
            _categoryLimitCollection.Insert(newLimit);
            limitId = newLimit.Id;
        }
        _categoryCollection.Insert(newCategory);
        var result = GetCreateCategoryResult(newCategory.Id, limitId);

        await _categoryCollection.SaveAsync(cancellationToken);
        LogCreateCategorySuccess(parameters.UserId, parameters.Name);
        return new Result<CreateCategoryResult, CategoryServiceResultCode>(result);
    }

    private async Task<CategoryServiceResultCode> UpdateChildCategoriesAsync(
        List<Guid> childrenIds,
        Guid userId,
        decimal? parentLimitAmount,
        CancellationToken cancellationToken
    )
    {
        if (!await _categoryCollection.ExistsEntriesAsync(childrenIds, userId, cancellationToken))
            return CategoryServiceResultCode.CHILD_CATEGORY_DOES_NOT_EXIST;
        var oldParents = await _categoryCollection.SelectByChildIdsAsync(
            childrenIds,
            userId,
            cancellationToken
        );
        RemoveChildIdFromParent([.. oldParents], childrenIds);
        if (parentLimitAmount is null)
            return CategoryServiceResultCode.OK;
        var childLimits = await _categoryLimitCollection.SelectCurrentLimitByCategoryIdAsync(
            childrenIds,
            cancellationToken
        );
        if (IsLimitAmountValid(parentLimitAmount.Value, [.. childLimits]))
            return CategoryServiceResultCode.OK;
        return CategoryServiceResultCode.INVALID_CATEGORY_LIMIT;
    }

    private async Task<CategoryServiceResultCode> UpdateParentCategoryAsync(
        Guid parentId,
        Guid newChildId,
        Guid userId,
        decimal? childLimitAmount,
        CancellationToken cancellationToken
    )
    {
        var parent = await _categoryCollection.SelectAsync(parentId, userId, cancellationToken);
        if (parent is null)
            return CategoryServiceResultCode.PARENT_CATEGORY_DOES_NOT_EXIST;
        parent.Children = parent.Children.Append(newChildId);
        if (childLimitAmount is null)
            return CategoryServiceResultCode.OK;
        var parentLimit = await _categoryLimitCollection.SelectCurrentLimitByCategoryIdAsync(
            parentId,
            cancellationToken
        );
        if (parentLimit is null)
            return CategoryServiceResultCode.OK;
        if (!NeedsNewLimitEntry(parentLimit))
        {
            parentLimit.Amount += childLimitAmount.Value;
            return CategoryServiceResultCode.OK;
        }
        var newParentLimit = new CategoryLimit
        {
            CategoryId = parentId,
            Amount = parentLimit.Amount + childLimitAmount.Value,
            Start = DateTime.UtcNow,
            Period = parentLimit.Period,
        };
        _categoryLimitCollection.Insert(newParentLimit);
        return CategoryServiceResultCode.OK;
    }

    private async Task<bool> ExistCycle(
        Guid startNode,
        Guid userId,
        List<Guid> nextNodes,
        CancellationToken cancellationToken
    )
    {
        var visited = new HashSet<Guid> { startNode };
        var toVisit = new List<Guid>(nextNodes);

        for (var index = 0; index < toVisit.Count; index++)
        {
            var id = toVisit[index];
            if (visited.Contains(id))
                return true;
            var category = await _categoryCollection.SelectAsync(id, userId, cancellationToken);
            if (category is not null)
                toVisit.AddRange(category.Children);
            visited.Add(id);
        }
        return false;
    }

    private CreateCategoryResult GetCreateCategoryResult(Guid categoryId, Guid? limitId)
    {
        var modifiedCategories = _categoryCollection.GetModifiedItems();
        var modifiedLimits = _categoryLimitCollection.GetModifiedItems();

        return new CreateCategoryResult
        {
            CreatedCategory = modifiedCategories.First(item => item.Id == categoryId),
            CreatedLimit = limitId is null
                ? null
                : modifiedLimits.First(item => item.Id == limitId.Value),
            ModifiedCategories = [.. modifiedCategories.Where(item => item.Id != categoryId)],
            ModifiedCategoryLimits = limitId is null
                ? []
                : [.. modifiedLimits.Where(item => item.Id != limitId.Value)],
        };
    }

    private static bool IsLimitAmountValid(decimal limit, List<CategoryLimit> childrenLimits)
    {
        var totalLimit = 0.0m;
        foreach (var childLimit in childrenLimits)
        {
            totalLimit += childLimit.Amount;
        }
        return totalLimit <= limit;
    }

    private static void RemoveChildIdFromParent(List<Category> parents, List<Guid> childrenIds)
    {
        foreach (var id in childrenIds)
        {
            var parent = parents.FirstOrDefault(item => item.Children.Contains(id));
            if (parent is null)
                continue;
            parent.Children = parent.Children.Where(childId => childId != id);
        }
    }

    private static bool NeedsNewLimitEntry(CategoryLimit limit)
    {
        var now = DateTime.UtcNow;
        var period = (int)limit.Period;
        return limit.Start.AddMonths(period) < now;
    }
}
