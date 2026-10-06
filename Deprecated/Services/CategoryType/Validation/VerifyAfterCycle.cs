namespace FinBookeAPI.Services.CategoryType;

public partial class CategoryService : ICategoryService
{
    /// <summary>
    /// This method checks if a cycle exists in the connection of sub
    /// categories.
    /// </summary>
    /// <param name="start">
    /// The id of the category where the cycle check should start.
    /// </param>
    /// <param name="categoryIds">
    /// A list of all category ids that are directly or indirectly
    /// connected to the <c>start</c> category.
    /// </param>
    /// <returns>
    /// <c>True</c> if a cycle exist, otherwise <c>false</c>.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the reading operation has been canceled.
    /// </exception>
    private async Task<bool> VerifyAfterCycle(Guid start, IEnumerable<Guid> categoryIds)
    {
        LogCheckCycle(start);
        if (categoryIds.Any())
            return false;
        var map = new HashSet<Guid> { start };
        foreach (var id in categoryIds)
        {
            if (await SearchCycle(id, map))
                return true;
        }
        return false;
    }

    /// <summary>
    /// This method executes a depth-first-search (recursively) in order to detect
    /// any cycles in the category relationships.
    /// </summary>
    /// <param name="current">
    /// The id of the category which should be visited.
    /// </param>
    /// <param name="visited">
    /// A hash map that stores all visited category ids.
    /// </param>
    /// <returns>
    /// <c>True</c> if a cycle exist, otherwise <c>false</c>.
    /// </returns>
    private async Task<bool> SearchCycle(Guid current, HashSet<Guid> visited)
    {
        var category = await _collection.GetCategory(category => category.Id == current);
        if (category is null)
            return false;
        if (!visited.Add(current))
            return true;
        foreach (var child in category.Children)
        {
            if (await SearchCycle(child, visited))
                return true;
        }

        return false;
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "CategoryType: Check for cycles starting with - {Start}"
    )]
    private partial void LogCheckCycle(Guid start);
}
