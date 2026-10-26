using FinBooKeApp.Data.Interfaces;
using FinBooKeApp.Data.Models;
using Moq;

namespace FinBookeApp.Tests.Mocks.Collections.Categories;

public static class MockCategoryCollection
{
    public static Mock<ICategoryCollection> GetMock(InMemoryCollection collection)
    {
        var mock = new Mock<ICategoryCollection>();

        mock.Setup(obj =>
                obj.ExistEntryAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (Guid id, Guid userId, CancellationToken cancellationToken) =>
                {
                    return collection.Categories.Any(item =>
                        item.Id == id && item.UserId == userId
                    );
                }
            );

        mock.Setup(obj =>
                obj.ExistEntryAsync(
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (string name, Guid userId, CancellationToken cancellationToken) =>
                {
                    return collection.Categories.Any(item =>
                        item.Name == name && item.UserId == userId
                    );
                }
            );

        mock.Setup(obj =>
                obj.ExistsEntriesAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (IEnumerable<Guid> ids, Guid userId, CancellationToken cancellationToken) =>
                {
                    return ids.All(id =>
                        collection.Categories.Any(item => item.Id == id && item.UserId == userId)
                    );
                }
            );

        mock.Setup(obj =>
                obj.SelectAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                (Guid id, Guid userId, CancellationToken cancellationToken) =>
                {
                    return collection.Categories.FirstOrDefault(item =>
                        item.Id == id && item.UserId == userId
                    );
                }
            );

        mock.Setup(obj =>
                obj.SelectByChildIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (IEnumerable<Guid> ids, Guid userId, CancellationToken cancellationToken) =>
                {
                    return collection.Categories.Where(item =>
                        item.Children.Any(id => ids.Contains(id))
                    );
                }
            );

        mock.Setup(obj => obj.Insert(It.IsAny<Category>()))
            .Callback((Category category) => collection.TrackedCategories.Add(category));

        mock.Setup(obj => obj.GetModifiedItems()).Returns(() => collection.TrackedCategories);

        mock.Setup(obj => obj.SaveAsync(It.IsAny<CancellationToken>()))
            .Callback(
                (CancellationToken cancellationToken) =>
                {
                    collection.CategoryLimits.AddRange(collection.TrackedCategoryLimits);
                    collection.Categories.AddRange(collection.TrackedCategories);
                    collection.TrackedCategories.Clear();
                    collection.TrackedCategoryLimits.Clear();
                }
            );

        return mock;
    }
}
