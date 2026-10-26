using FinBooKeApp.Data.Interfaces;
using FinBooKeApp.Data.Models;
using FinBooKeApp.Tests.Mocks.Collections.Categories;
using Moq;

namespace FinBooKeApp.Tests.Mocks.Collections.Categories;

public static class MockCategoryLimitCollection
{
    public static Mock<ICategoryLimitCollection> GetMock(InMemoryCollection collection)
    {
        var mock = new Mock<ICategoryLimitCollection>();

        mock.Setup(obj =>
                obj.SelectCurrentLimitByCategoryIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (Guid id, CancellationToken cancellationToken) =>
                {
                    return collection.CategoryLimits.FirstOrDefault(item => item.CategoryId == id);
                }
            );

        mock.Setup(obj =>
                obj.SelectCurrentLimitByCategoryIdAsync(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (List<Guid> ids, CancellationToken cancellationToken) =>
                {
                    return collection.CategoryLimits.Where(item => ids.Contains(item.CategoryId));
                }
            );

        mock.Setup(obj => obj.Insert(It.IsAny<CategoryLimit>()))
            .Callback((CategoryLimit limit) => collection.TrackedCategoryLimits.Add(limit));

        mock.Setup(obj => obj.GetModifiedItems())
            .Returns(() =>
            {
                return collection.TrackedCategoryLimits;
            });

        mock.Setup(obj => obj.SaveAsync(It.IsAny<CancellationToken>()))
            .Callback(
                (CancellationToken cancellationToken) =>
                {
                    collection.CategoryLimits.AddRange(collection.TrackedCategoryLimits);
                    collection.TrackedCategoryLimits.Clear();
                }
            );

        return mock;
    }
}
