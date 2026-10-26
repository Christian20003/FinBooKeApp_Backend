using FinBooKeApp.Data.Models;

namespace FinBookeApp.Tests.Mocks.Collections.Categories;

public class InMemoryCollection
{
    public readonly List<Category> Categories;
    public readonly List<CategoryLimit> CategoryLimits;
    public readonly Guid UserId = Guid.NewGuid();

    public readonly List<Category> TrackedCategories = [];
    public readonly List<CategoryLimit> TrackedCategoryLimits = [];

    public InMemoryCollection()
    {
        var ids = new List<Guid>();
        for (var i = 0; i < 5; i++)
            ids.Add(Guid.NewGuid());
        Categories =
        [
            new()
            {
                Id = ids.ElementAt(0),
                UserId = UserId,
                Name = "TestCategory",
                Color = "#ffffff",
                Children = [ids.ElementAt(1), ids.ElementAt(2)],
            },
            new()
            {
                Id = ids.ElementAt(1),
                UserId = UserId,
                Name = "TestSubCategory1",
                Color = "#000000",
            },
            new()
            {
                Id = ids.ElementAt(2),
                UserId = UserId,
                Name = "TestSubCategory2",
                Color = "#aaaaaa",
            },
        ];

        CategoryLimits =
        [
            new()
            {
                CategoryId = ids.ElementAt(0),
                Amount = 500.0m,
                Start = DateTime.UtcNow,
                Period = TimePeriod.ONE_MONTH,
            },
            new()
            {
                CategoryId = ids.ElementAt(1),
                Amount = 200.0m,
                Start = DateTime.UtcNow.AddDays(-10),
                Period = TimePeriod.THREE_MONTHS,
            },
            new()
            {
                CategoryId = ids.ElementAt(2),
                Amount = 100.0m,
                Start = DateTime.UtcNow.AddDays(-2),
                Period = TimePeriod.FOUR_MONTHS,
            },
        ];
    }

    public Category GetParentCategory()
    {
        return Categories.First(item => item.Children.Any());
    }

    public Category GetChildCategory()
    {
        return Categories.First(item => !item.Children.Any());
    }

    public CategoryLimit GetLimit(Guid categoryId)
    {
        return CategoryLimits.First(item => item.CategoryId == categoryId);
    }
}
