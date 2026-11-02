using FinBooKeApp.Data.Interfaces;
using FinBooKeApp.Data.Models;
using Moq;

namespace FinBooKeApp.Tests.Mocks.Collections.Categories;

public static class MockCategoryCollection
{
    public static Mock<ICategoryCollection> GetMock(InMemoryCollection collection)
    {
        var mock = new Mock<ICategoryCollection>();

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
                obj.SelectAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                (string name, Guid userId, CancellationToken cancellationToken) =>
                {
                    return collection.Categories.FirstOrDefault(item =>
                        item.Name == name && item.UserId == userId
                    );
                }
            );

        mock.Setup(obj =>
                obj.SelectParentAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (Guid id, Guid userId, CancellationToken cancellationToken) =>
                {
                    return collection.Categories.FirstOrDefault(item =>
                        item.Children.Contains(id) && item.UserId == userId
                    );
                }
            );

        mock.Setup(obj => obj.Insert(It.IsAny<Category>()))
            .Callback((Category category) => collection.TrackedCategories.Add(category));

        mock.Setup(obj => obj.Update(It.IsAny<Category>()))
            .Callback((Category category) => collection.TrackedCategories.Add(category));

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
