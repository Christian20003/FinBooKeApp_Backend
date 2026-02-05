using FinBookeAPI.Models.Category;

namespace FinBookeAPI.Tests.Records;

public static class CategoryRecord
{
    public static CategoryTag GetObject()
    {
        return new CategoryTag
        {
            Name = "Living",
            Color = "rgb(66,55,88)",
            UserId = Guid.NewGuid(),
            Children = [],
            Limit = new Limit { Amount = 50.0M, PeriodDays = 30 },
        };
    }

    public static List<CategoryTag> GetObjects()
    {
        var userId = Guid.NewGuid();
        var firstChild = Guid.NewGuid();
        var secondChild = Guid.NewGuid();
        return
        [
            new CategoryTag
            {
                UserId = Guid.NewGuid(),
                Name = "Hobbys",
                Color = "rgb(77,44,33)",
                Limit = new Limit { Amount = 50.0M, PeriodDays = 30 },
            },
            new CategoryTag
            {
                UserId = userId,
                Name = "apartment",
                Color = "rgb(62,69,32)",
                Children = [firstChild, secondChild],
                Limit = new Limit { Amount = 250.0M, PeriodDays = 30 },
            },
            new CategoryTag
            {
                UserId = userId,
                Name = "electricity",
                Id = firstChild,
                Color = "rgb(235,64,14)",
                Limit = new Limit { Amount = 200.0M, PeriodDays = 30 },
            },
            new CategoryTag
            {
                UserId = userId,
                Name = "heating",
                Id = secondChild,
                Color = "rgb(235,164,14)",
                Limit = new Limit { Amount = 25.0M, PeriodDays = 30 },
            },
        ];
    }
}
